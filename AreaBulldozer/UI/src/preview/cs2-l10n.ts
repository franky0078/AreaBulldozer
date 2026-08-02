export function useLocalization() {
  return {
    translate: (_key: string, fallback?: string) => fallback ?? _key,
  };
}
