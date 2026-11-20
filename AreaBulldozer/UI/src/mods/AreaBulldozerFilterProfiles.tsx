import React, { useCallback, useEffect, useRef, useState } from "react";
import { bindValue, trigger } from "cs2/api";
import mod from "mod.json";

import { BindingKeys } from "../bindings";
import styles from "./AreaBulldozerCompact.module.scss";
import { useSafeValue } from "./useSafeValue";
import { AreaBulldozerTooltip } from "./AreaBulldozerTooltip";

const profiles$ = bindValue<string>(mod.id, "filterProfiles", "");
const activeProfile$ = bindValue<string>(mod.id, "activeFilterProfile", "");
const profileModified$ = bindValue<boolean>(mod.id, "filterProfileModified", false);

type Editor = "create" | "rename" | "delete";

interface Props {
    text: (key: string, fallback: string) => string;
}

export function AreaBulldozerFilterProfiles({ text }: Props) {
    const data = useSafeValue("filterProfiles$", profiles$, "");
    const activeId = useSafeValue("activeFilterProfile$", activeProfile$, "");
    const modified = useSafeValue("filterProfileModified$", profileModified$, false);
    const controlRef = useRef<HTMLDivElement>(null);
    const pointerInside = useRef(false);
    const [expanded, setExpanded] = useState(false);
    const [editor, setEditor] = useState<Editor | null>(null);
    const [draft, setDraft] = useState("");

    const builtIn = [
        { id: "deadTrees", name: text("ProfileDeadTrees", "Tote Bäume entfernen") },
        { id: "vegetation", name: text("ProfileVegetation", "Fläche von Vegetation freimachen") },
        { id: "missingAssets", name: text("ProfileMissing", "Fehlende Assets aufräumen") },
        { id: "looseProps", name: text("ProfileProps", "Lose Deko entfernen") },
        { id: "undergroundRail", name: text("ProfileRail", "Unterirdische Gleise entfernen") },
    ];
    const custom = React.useMemo(() => data.split("\n")
        .map(row => row.split("\t"))
        .filter(([id, name, filters]) => /^[0-9a-f]{32}$/i.test(id) && !!name && /^\d+$/.test(filters ?? ""))
        .map(([id, name]) => ({ id, name })), [data]);

    const activeCustom = custom.find(profile => profile.id === activeId);
    const current = activeCustom ?? builtIn.find(profile => profile.id === activeId);
    const currentName = current?.name ?? text("ProfileCurrent", "Aktuelle Filter");
    const selectedLabel = modified ? `${currentName} · ${text("ProfileModified", "Geändert")}` : currentName;
    const profileName = draft.trim();
    const duplicate = custom.some(profile =>
        (editor !== "rename" || profile.id !== activeId) &&
        profile.name.toUpperCase() === profileName.toUpperCase()
    );
    const invalidName = profileName.length === 0 || duplicate || /[\u0000-\u001f\u007f-\u009f]/.test(profileName);

    const setPointerInside = useCallback((inside: boolean) => {
        if (pointerInside.current === inside) {
            return;
        }

        pointerInside.current = inside;
        trigger(mod.id, BindingKeys.setPointerOverUI, inside);
    }, []);

    useEffect(() => {
        // The game UI can remove the clicked row without delivering mouseleave.
        const releaseOutside = (event: MouseEvent) => {
            if (pointerInside.current && !controlRef.current?.contains(event.target as Node | null)) {
                setPointerInside(false);
            }
        };
        document.addEventListener("mousemove", releaseOutside, true);
        document.addEventListener("mousedown", releaseOutside, true);
        return () => {
            document.removeEventListener("mousemove", releaseOutside, true);
            document.removeEventListener("mousedown", releaseOutside, true);
            setPointerInside(false);
        };
    }, [setPointerInside]);

    const closeMenu = () => {
        setPointerInside(false);
        setEditor(null);
        setExpanded(false);
    };

    const choose = (id: string) => {
        closeMenu();
        trigger(mod.id, "loadFilterProfile", id);
    };

    const startEditor = (mode: Editor) => {
        setDraft(mode === "rename" ? activeCustom?.name ?? "" : "");
        setEditor(mode);
    };

    const save = () => {
        if (invalidName) {
            return;
        }
        if (editor === "create") {
            trigger(mod.id, "createFilterProfile", profileName);
        } else if (editor === "rename" && activeCustom) {
            trigger(mod.id, "renameFilterProfile", activeId, profileName);
        }
        closeMenu();
    };

    const choices = (profiles: typeof builtIn) => profiles.map(profile => (
        <button key={profile.id} type="button" aria-pressed={profile.id === activeId}
            className={styles.profileChoice} onClick={() => choose(profile.id)}>
            {profile.name}
        </button>
    ));

    return (
        <div ref={controlRef} className={styles.profileControl}
            onMouseEnter={() => setPointerInside(true)}
            onMouseLeave={() => setPointerInside(false)}
            onMouseDown={event => event.stopPropagation()}
            onMouseUp={event => event.stopPropagation()}
            onClick={event => event.stopPropagation()}>
            <AreaBulldozerTooltip title={selectedLabel}>
            <button type="button" className={styles.profileToggle}
                aria-expanded={expanded} onClick={() => {
                    if (expanded) {
                        closeMenu();
                    } else {
                        setPointerInside(true);
                        setExpanded(true);
                    }
                }}>
                <span>{selectedLabel}</span>
                <svg viewBox="0 0 16 16" aria-hidden="true"><path d="m3 6 5 5 5-5" /></svg>
            </button></AreaBulldozerTooltip>
            {expanded && (
                <div className={styles.profileMenu}>
                    <div className={styles.profileChoices}>
                        <button type="button" className={styles.profileChoice} aria-pressed={!activeId}
                            onClick={() => choose("")}>
                            {text("ProfileCurrent", "Aktuelle Filter")}
                        </button>
                        {custom.length > 0 && <>
                            <div className={styles.profileGroup}>{text("ProfileCustom", "Eigene Profile")}</div>
                            {choices(custom)}
                        </>}
                        <div className={styles.profileGroup}>{text("ProfileBuiltIn", "Vorgefertigt")}</div>
                        {choices(builtIn)}
                    </div>
                    <div className={styles.profileActions}>
                        <button type="button" onClick={() => startEditor("create")}>{text("ProfileCreate", "Als Profil speichern")}</button>
                        {activeCustom && <>
                            <button type="button" disabled={!modified}
                                onClick={() => trigger(mod.id, "updateFilterProfile", activeId)}>
                                {text("ProfileUpdate", "Profil aktualisieren")}
                            </button>
                            <button type="button" onClick={() => startEditor("rename")}>
                                {text("ProfileRename", "Umbenennen")}
                            </button>
                            <button type="button" onClick={() => startEditor("delete")}>
                                {text("ProfileDelete", "Löschen")}
                            </button>
                        </>}
                    </div>
                    {editor === "delete" && activeCustom && (
                        <div className={styles.profileEditor}>
                            <span>{text("ProfileDeletePrompt", "Dieses eigene Profil löschen?")}</span>
                            <div className={styles.profileActions}>
                                <button type="button" onClick={() => {
                                    trigger(mod.id, "deleteFilterProfile", activeId);
                                    closeMenu();
                                }}>{text("ProfileDelete", "Löschen")}</button>
                                <button type="button" onClick={() => setEditor(null)}>{text("ProfileCancel", "Abbrechen")}</button>
                            </div>
                        </div>
                    )}
                    {(editor === "create" || editor === "rename") && (
                        <div className={styles.profileEditor}>
                            <input type="text" maxLength={40} value={draft}
                                aria-label={text("ProfileName", "Profilname")}
                                placeholder={text("ProfileName", "Profilname")}
                                onChange={event => setDraft(event.target.value)}
                                onKeyDown={event => {
                                    event.stopPropagation();
                                    if (event.key === "Enter") {
                                        event.preventDefault();
                                        save();
                                    }
                                }} />
                            {duplicate && <span role="status">{text("ProfileDuplicate", "Der Name ist bereits vergeben.")}</span>}
                            <div className={styles.profileActions}>
                                <button type="button" disabled={invalidName} onClick={save}>{text("ProfileSave", "Speichern")}</button>
                                <button type="button" onClick={() => setEditor(null)}>{text("ProfileCancel", "Abbrechen")}</button>
                            </div>
                        </div>
                    )}
                </div>
            )}
        </div>
    );
}
