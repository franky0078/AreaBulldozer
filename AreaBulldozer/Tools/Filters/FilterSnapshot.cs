using System;

namespace AreaBulldozer.Tools
{


    public readonly struct FilterSnapshot
        : IEquatable<FilterSnapshot>
    {

        // Primäre Objektfilter
        public readonly bool DeleteTrees;
        public readonly bool DeleteBuildings;
        public readonly bool DeleteRoads;
        public readonly bool DeletePaths;
        public readonly bool DeleteRailways;
        public readonly bool DeleteSurfaces;
        public readonly bool DeleteStaticObjects;

        // Unterkategorien statischer Objekte und Marker
        public readonly bool DeleteGeneralProps;
        public readonly bool DeleteStreetLights;
        public readonly bool DeleteQuantityObjects;
        public readonly bool DeleteBrandingObjects;
        public readonly bool DeleteActivityLocations;
        public readonly bool DeleteSpawnLocations;
        public readonly bool DeleteMarkerNetworks;


        // Besitz- und Unterobjekt-Sicherheitsregeln
        public readonly bool DeleteBuildingSubObjects;
        public readonly bool DeleteNetworkSubObjects;
        public readonly bool ProtectOwnedObjects;

        private FilterSnapshot(
            Setting settings)
        {
            DeleteTrees =
                settings.DeleteTrees;

            DeleteBuildings =
                settings.DeleteBuildings;

            DeleteRoads =
                settings.DeleteRoads;

            DeletePaths =
                settings.DeletePaths;

            DeleteRailways =
                settings.DeleteRailways;

            DeleteSurfaces =
                settings.DeleteSurfaces;

            DeleteStaticObjects =
                settings.DeleteStaticObjects;

            DeleteGeneralProps =
                settings.DeleteGeneralProps;

            DeleteStreetLights =
                settings.DeleteStreetLights;

            DeleteQuantityObjects =
                settings.DeleteQuantityObjects;

            DeleteBrandingObjects =
                settings.DeleteBrandingObjects;

            DeleteActivityLocations =
                settings.DeleteActivityLocations;

            DeleteSpawnLocations =
                settings.DeleteSpawnLocations;

            DeleteMarkerNetworks =
                settings.DeleteMarkerNetworks;

            DeleteBuildingSubObjects =
                settings.DeleteBuildingSubObjects;

            DeleteNetworkSubObjects =
                settings.DeleteNetworkSubObjects;

            ProtectOwnedObjects =
                settings.ProtectOwnedObjects;
        }

        public static FilterSnapshot FromSettings(
            Setting settings)
        {
            return settings == null
                ? default
                : new FilterSnapshot(settings);
        }

        public bool HasAnyPrimaryFilter =>
            DeleteTrees ||
            DeleteBuildings ||
            DeleteRoads ||
            DeletePaths ||
            DeleteRailways ||
            DeleteSurfaces ||
            DeleteStaticObjects;

        public bool Equals(
            FilterSnapshot other)
        {
            return
                DeleteTrees == other.DeleteTrees &&
                DeleteBuildings == other.DeleteBuildings &&
                DeleteRoads == other.DeleteRoads &&
                DeletePaths == other.DeletePaths &&
                DeleteRailways == other.DeleteRailways &&
                DeleteSurfaces == other.DeleteSurfaces &&
                DeleteStaticObjects == other.DeleteStaticObjects &&
                DeleteGeneralProps == other.DeleteGeneralProps &&
                DeleteStreetLights == other.DeleteStreetLights &&
                DeleteQuantityObjects == other.DeleteQuantityObjects &&
                DeleteBrandingObjects == other.DeleteBrandingObjects &&
                DeleteActivityLocations == other.DeleteActivityLocations &&
                DeleteSpawnLocations == other.DeleteSpawnLocations &&
                DeleteMarkerNetworks == other.DeleteMarkerNetworks &&
                DeleteBuildingSubObjects == other.DeleteBuildingSubObjects &&
                DeleteNetworkSubObjects == other.DeleteNetworkSubObjects &&
                ProtectOwnedObjects == other.ProtectOwnedObjects;
        }

        public override bool Equals(
            object obj)
        {
            return obj is FilterSnapshot other &&
                   Equals(other);
        }

        public override int GetHashCode()
        {
            int hash = 0;

            hash |= DeleteTrees ? 1 << 0 : 0;
            hash |= DeleteBuildings ? 1 << 1 : 0;
            hash |= DeleteRoads ? 1 << 2 : 0;
            hash |= DeletePaths ? 1 << 3 : 0;
            hash |= DeleteRailways ? 1 << 4 : 0;
            hash |= DeleteSurfaces ? 1 << 5 : 0;
            hash |= DeleteStaticObjects ? 1 << 6 : 0;
            hash |= DeleteGeneralProps ? 1 << 7 : 0;
            hash |= DeleteStreetLights ? 1 << 8 : 0;
            hash |= DeleteQuantityObjects ? 1 << 9 : 0;
            hash |= DeleteBrandingObjects ? 1 << 10 : 0;
            hash |= DeleteActivityLocations ? 1 << 11 : 0;
            hash |= DeleteSpawnLocations ? 1 << 12 : 0;
            hash |= DeleteMarkerNetworks ? 1 << 13 : 0;
            hash |= DeleteBuildingSubObjects ? 1 << 14 : 0;
            hash |= DeleteNetworkSubObjects ? 1 << 15 : 0;
            hash |= ProtectOwnedObjects ? 1 << 16 : 0;

            return hash;
        }

        public static bool operator ==(
            FilterSnapshot left,
            FilterSnapshot right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(
            FilterSnapshot left,
            FilterSnapshot right)
        {
            return !left.Equals(right);
        }
    }
}
