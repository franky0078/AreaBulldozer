declare module "cs2/api" {
  export interface ValueBinding<T> {
    readonly value: T;
    subscribe(listener?: (value: T) => void): {
      readonly value: T;
      dispose(): void;
    };
    dispose(): void;
  }

  export function bindValue<T>(
    group: string,
    name: string,
    fallbackValue?: T
  ): ValueBinding<T>;

  export function useValue<T>(binding: ValueBinding<T>): T;

  export function trigger(
    group: string,
    name: string,
    ...args: unknown[]
  ): void;
}
