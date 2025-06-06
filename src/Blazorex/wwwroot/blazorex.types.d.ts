// Modern TypeScript type definitions for Blazorex Canvas Library

export type CanvasContextOptions = {
    readonly alpha?: boolean;
    readonly desynchronized?: boolean;
    readonly colorSpace?: PredefinedColorSpace;
    readonly willReadFrequently?: boolean;
};

export type ContextInfo = {
    readonly id: string;
    context: CanvasRenderingContext2D;
    readonly managedInstance: DotNetObjectReference;
    readonly contextOptions: CanvasContextOptions;
};

// C# MarshalReference equivalent - matches MarshalReference.cs structure
export type MarshalReference = {
    readonly id: number;
    readonly isElementRef: boolean;
    readonly classInitializer?: string;
};

// Property value wrapper types from Blazor
export type PropertyValueWrapper = {
    readonly value: unknown;
} | {
    readonly id: number;
} | {
    readonly result: unknown;
};

// Direct property values that can come from Blazor
export type PropertyValue = string | number | Date | PropertyValueWrapper;

export type EventCoordinates = {
    readonly clientX: number;
    readonly clientY: number;
    readonly button?: number;
};

export type MouseEventCoordinates = {
    clientX: number;
    clientY: number;
    offsetX: number;
    offsetY: number;
};

export type WheelEventData = EventCoordinates & {
    readonly deltaX: number;
    readonly deltaY: number;
};

export type ModifierState = {
    shift: boolean;
    ctrl: boolean;
    alt: boolean;
    meta: boolean;
};

export type KeyEventData = {
    readonly keyCode: number;
    readonly key: string;
    readonly isHeld: boolean;
    readonly heldKeys: readonly number[];
    readonly modifiers: Readonly<ModifierState>;
};

export type BatchOperation = {
    readonly methodName: string;
    readonly args: unknown;
    readonly isProperty: boolean;
};

export type DotNetObjectReference = {
    invokeMethodAsync<T = void>(methodName: string, ...args: unknown[]): Promise<T>;
};

export type EventType = 'wheel' | 'mousedown' | 'mouseup' | 'touchstart' | 'touchend';
export type EventHandler = (event: Event) => void;

export type EventExtractors = {
    readonly wheel: (e: WheelEvent) => WheelEventData;
    readonly mousedown: (e: MouseEvent) => EventCoordinates;
    readonly mouseup: (e: MouseEvent) => EventCoordinates;
    readonly touchstart: (e: TouchEvent) => EventCoordinates;
    readonly touchend: (e: TouchEvent) => EventCoordinates;
};

export type EventMethodMap = {
    readonly wheel: 'Wheel';
    readonly mousedown: 'MouseDown';
    readonly mouseup: 'MouseUp';
    readonly touchstart: 'MouseDown';
    readonly touchend: 'MouseUp';
};

export type BlazorexAPI = {
    readonly initCanvas: (id: string, managedInstance: DotNetObjectReference, contextOptions?: CanvasContextOptions) => void;
    readonly onFrameUpdate: (timeStamp: number) => void;
    readonly createImageData: (ctxId: string, width: number, height: number) => number | null;
    readonly putImageData: (ctxId: string, imageId: number, data: Uint8ClampedArray, x: number, y: number) => void;
    readonly processBatch: (ctxId: string, jsonBatch: readonly BatchOperation[]) => void;
    readonly directCall: (ctxId: string, methodName: string, jParams?: unknown[]) => unknown;
    readonly removeContext: (ctxId: string) => boolean;
    readonly resizeCanvas: (ctxId: string, width: number, height: number) => void;
};

export type CreateBlazorexAPI = () => BlazorexAPI; 