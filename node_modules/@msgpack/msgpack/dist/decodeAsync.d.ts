import type { ReadableStreamLike } from "./utils/stream";
import type { DecodeOptions } from "./decode";
import type { SplitUndefined } from "./context";
export declare function decodeAsync<ContextType>(streamLike: ReadableStreamLike<ArrayLike<number> | BufferSource>, options?: DecodeOptions<SplitUndefined<ContextType>>): Promise<unknown>;
export declare function decodeArrayStream<ContextType>(streamLike: ReadableStreamLike<ArrayLike<number> | BufferSource>, options?: DecodeOptions<SplitUndefined<ContextType>>): AsyncGenerator<unknown, void, unknown>;
export declare function decodeMultiStream<ContextType>(streamLike: ReadableStreamLike<ArrayLike<number> | BufferSource>, options?: DecodeOptions<SplitUndefined<ContextType>>): AsyncGenerator<unknown, void, unknown>;
/**
 * @deprecated Use {@link decodeMultiStream()} instead.
 */
export declare function decodeStream<ContextType>(streamLike: ReadableStreamLike<ArrayLike<number> | BufferSource>, options?: DecodeOptions<SplitUndefined<ContextType>>): AsyncGenerator<unknown, void, unknown>;
