import { useSyncExternalStore } from "react";

type Listener = () => void;

export interface PreviewBinding<T> {
  readonly modId: string;
  readonly name: string;
  readonly defaultValue: T;
}

const values = new Map<string, unknown>();
const listeners = new Set<Listener>();

function bindingKey(modId: string, name: string) {
  return `${modId}:${name}`;
}

function notify() {
  for (const listener of listeners) {
    listener();
  }
}

function subscribe(listener: Listener) {
  listeners.add(listener);
  return () => listeners.delete(listener);
}

function readValue<T>(binding: PreviewBinding<T>): T {
  const key = bindingKey(binding.modId, binding.name);
  return (values.has(key) ? values.get(key) : binding.defaultValue) as T;
}

function setValue(modId: string, name: string, value: unknown) {
  values.set(bindingKey(modId, name), value);
  notify();
}

export function bindValue<T>(
  modId: string,
  name: string,
  defaultValue: T
): PreviewBinding<T> {
  const key = bindingKey(modId, name);

  if (!values.has(key)) {
    values.set(key, defaultValue);
  }

  return { modId, name, defaultValue };
}

export function useValue<T>(binding: PreviewBinding<T>): T {
  return useSyncExternalStore(
    subscribe,
    () => readValue(binding),
    () => readValue(binding)
  );
}

function lowerFirst(value: string) {
  return value.length === 0
    ? value
    : value.charAt(0).toLowerCase() + value.slice(1);
}

export function trigger(
  modId: string,
  triggerName: string,
  value?: unknown
) {
  if (triggerName === "toggleTool") {
    const key = bindingKey(modId, "isToolActive");
    setValue(modId, "isToolActive", !(values.get(key) as boolean));
    return;
  }

  if (triggerName === "deactivateTool") {
    setValue(modId, "isToolActive", false);
    return;
  }

  if (triggerName === "setPointerOverUI") {
    return;
  }

  if (triggerName.startsWith("set")) {
    const bindingName = lowerFirst(triggerName.slice(3));
    setValue(modId, bindingName, value);
    return;
  }

  console.info("[AreaBulldozer Preview] Unhandled trigger", {
    modId,
    triggerName,
    value,
  });
}

export function setPreviewValue(
  modId: string,
  bindingName: string,
  value: unknown
) {
  setValue(modId, bindingName, value);
}
