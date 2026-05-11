import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { renderHook, act, waitFor } from "@testing-library/react";
import useApiResource from "./useApiResource";
import { API_BASE_URL } from "../config/config";

describe("useApiResource", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    global.fetch = vi.fn();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("should initialize with default state when autoLoad is false", () => {
    const { result } = renderHook(() =>
      useApiResource("/api/test", { autoLoad: false }),
    );

    expect(result.current.data).toEqual([]);
    expect(result.current.loading).toBe(false);
    expect(result.current.error).toBeNull();
    expect(global.fetch).not.toHaveBeenCalled();
  });

  it("should auto load data on mount when autoLoad is true", async () => {
    const mockData = [{ id: 1, name: "Item 1" }];

    global.fetch.mockResolvedValueOnce({
      ok: true,
      status: 200,
      headers: {
        get: vi.fn().mockReturnValue("application/json"),
      },
      text: vi.fn().mockResolvedValue(JSON.stringify(mockData)),
    });

    const { result } = renderHook(() =>
      useApiResource("/api/test", { autoLoad: true }),
    );

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(global.fetch).toHaveBeenCalledWith(`${API_BASE_URL}/api/test`, {
      headers: {
        "Content-Type": "application/json",
      },
    });

    expect(result.current.data).toEqual(mockData);
    expect(result.current.error).toBeNull();
  });

  it("should perform GET request and update data", async () => {
    const mockData = [{ id: 1, name: "Item 1" }];

    global.fetch.mockResolvedValueOnce({
      ok: true,
      status: 200,
      headers: {
        get: vi.fn().mockReturnValue("application/json"),
      },
      text: vi.fn().mockResolvedValue(JSON.stringify(mockData)),
    });

    const { result } = renderHook(() =>
      useApiResource("/api/test", { autoLoad: false }),
    );

    let responseData;
    await act(async () => {
      responseData = await result.current.request("/api/test", {
        method: "GET",
      });
    });

    expect(global.fetch).toHaveBeenCalledWith(`${API_BASE_URL}/api/test`, {
      headers: {
        "Content-Type": "application/json",
      },
      method: "GET",
    });

    expect(responseData).toEqual(mockData);
    expect(result.current.data).toEqual(mockData);
    expect(result.current.error).toBeNull();
  });

  it("should perform POST request without updating data by default", async () => {
    const mockResponse = { id: 2, name: "Created Item" };

    global.fetch.mockResolvedValueOnce({
      ok: true,
      status: 200,
      headers: {
        get: vi.fn().mockReturnValue("application/json"),
      },
      text: vi.fn().mockResolvedValue(JSON.stringify(mockResponse)),
    });

    const { result } = renderHook(() =>
      useApiResource("/api/test", { autoLoad: false }),
    );

    let responseData;
    await act(async () => {
      responseData = await result.current.request("/api/test", {
        method: "POST",
        body: JSON.stringify({ name: "Created Item" }),
      });
    });

    expect(responseData).toEqual(mockResponse);
    expect(result.current.data).toEqual([]);
  });

  it("should perform POST request and update data when updateState is true", async () => {
    const mockResponse = { id: 2, name: "Created Item" };

    global.fetch.mockResolvedValueOnce({
      ok: true,
      status: 200,
      headers: {
        get: vi.fn().mockReturnValue("application/json"),
      },
      text: vi.fn().mockResolvedValue(JSON.stringify(mockResponse)),
    });

    const { result } = renderHook(() =>
      useApiResource("/api/test", { autoLoad: false }),
    );

    await act(async () => {
      await result.current.request("/api/test", {
        method: "POST",
        body: JSON.stringify({ name: "Created Item" }),
        updateState: true,
      });
    });

    expect(result.current.data).toEqual(mockResponse);
  });

  it("should reload and always update data", async () => {
    const mockData = [{ id: 1, name: "Reloaded Item" }];

    global.fetch.mockResolvedValueOnce({
      ok: true,
      status: 200,
      headers: {
        get: vi.fn().mockReturnValue("application/json"),
      },
      text: vi.fn().mockResolvedValue(JSON.stringify(mockData)),
    });

    const { result } = renderHook(() =>
      useApiResource("/api/test", { autoLoad: false }),
    );

    await act(async () => {
      await result.current.reload();
    });

    expect(result.current.data).toEqual(mockData);
  });

  it("should support reload with overridePath", async () => {
    const mockData = [{ id: 99, name: "Override" }];

    global.fetch.mockResolvedValueOnce({
      ok: true,
      status: 200,
      headers: {
        get: vi.fn().mockReturnValue("application/json"),
      },
      text: vi.fn().mockResolvedValue(JSON.stringify(mockData)),
    });

    const { result } = renderHook(() =>
      useApiResource("/api/test", { autoLoad: false }),
    );

    await act(async () => {
      await result.current.reload("/api/other");
    });

    expect(global.fetch).toHaveBeenCalledWith(`${API_BASE_URL}/api/other`, {
      headers: {
        "Content-Type": "application/json",
      },
    });

    expect(result.current.data).toEqual(mockData);
  });

  it("should set error and throw when response is not ok", async () => {
    global.fetch.mockResolvedValueOnce({
      ok: false,
      status: 400,
      headers: {
        get: vi.fn(),
      },
      text: vi.fn(),
    });

    const { result } = renderHook(() =>
      useApiResource("/api/test", { autoLoad: false }),
    );

    await act(async () => {
      await expect(
        result.current.request("/api/test", { method: "GET" }),
      ).rejects.toThrow("GET /api/test failed with status 400");
    });

    await waitFor(() => {
      expect(result.current.error).toBeInstanceOf(Error);
    });

    expect(result.current.error.message).toBe(
      "GET /api/test failed with status 400",
    );
    expect(result.current.loading).toBe(false);
  });

  it("should return null for 204 responses", async () => {
    global.fetch.mockResolvedValueOnce({
      ok: true,
      status: 204,
      headers: {
        get: vi.fn(),
      },
      text: vi.fn(),
    });

    const { result } = renderHook(() =>
      useApiResource("/api/test", { autoLoad: false }),
    );

    let responseData;
    await act(async () => {
      responseData = await result.current.request("/api/test", {
        method: "DELETE",
      });
    });

    expect(responseData).toBeNull();
  });

  it("should return text when content type is not application/json", async () => {
    global.fetch.mockResolvedValueOnce({
      ok: true,
      status: 200,
      headers: {
        get: vi.fn().mockReturnValue("text/plain"),
      },
      text: vi.fn().mockResolvedValue("plain text response"),
    });

    const { result } = renderHook(() =>
      useApiResource("/api/test", { autoLoad: false }),
    );

    let responseData;
    await act(async () => {
      responseData = await result.current.request("/api/test", {
        method: "GET",
      });
    });

    expect(responseData).toBe("plain text response");
    expect(result.current.data).toBe("plain text response");
  });

  it("should set loading true while request is pending and false after completion", async () => {
    let resolveFetch;

    global.fetch.mockImplementation(
      () =>
        new Promise((resolve) => {
          resolveFetch = resolve;
        }),
    );

    const { result } = renderHook(() =>
      useApiResource("/api/test", { autoLoad: false }),
    );

    act(() => {
      result.current.request("/api/test", { method: "GET" });
    });

    expect(result.current.loading).toBe(true);

    await act(async () => {
      resolveFetch({
        ok: true,
        status: 200,
        headers: {
          get: vi.fn().mockReturnValue("application/json"),
        },
        text: vi.fn().mockResolvedValue(JSON.stringify([{ id: 1 }])),
      });
    });

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });
  });
});
