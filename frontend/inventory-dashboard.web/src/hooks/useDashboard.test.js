import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act } from "@testing-library/react";
import useDashboard from "./useDashboard";
import useApiResource from "./useApiResource";

vi.mock("./useApiResource", () => ({
  default: vi.fn(),
}));

const mockReloadOverview = vi.fn();
const mockRequest = vi.fn();
const mockSetOverview = vi.fn();

const ApiError = new Error("API Error");
const NotFoundError = { status: 404, message: "Not Found" };

describe("useDashboard", () => {
  beforeEach(() => {
    vi.clearAllMocks();

    useApiResource.mockReturnValue({
      data: {
        totalProducts: 25,
        totalCategories: 5,
        totalSuppliers: 8,
        lowStockCount: 3,
      },
      loading: false,
      error: null,
      reload: mockReloadOverview,
      request: mockRequest,
      setData: mockSetOverview,
    });
  });

  it("should return initial dashboard state", () => {
    const { result } = renderHook(() => useDashboard());

    expect(result.current.overview).toEqual({
      totalProducts: 25,
      totalCategories: 5,
      totalSuppliers: 8,
      lowStockCount: 3,
    });
    expect(result.current.loadingOverview).toBe(false);
    expect(result.current.overviewError).toBeNull();
    expect(result.current.reloadOverview).toBe(mockReloadOverview);
    expect(result.current.setOverview).toBe(mockSetOverview);
  });

  describe("loadOverview", () => {
    it("should call reloadOverview with dashboard overview endpoint", async () => {
      mockReloadOverview.mockResolvedValueOnce({
        totalProducts: 25,
        totalCategories: 5,
        totalSuppliers: 8,
        lowStockCount: 3,
      });

      const { result } = renderHook(() => useDashboard());

      await act(async () => {
        await result.current.loadOverview();
      });

      expect(mockReloadOverview).toHaveBeenCalledWith(
        "/api/Dashboard/overview",
      );
    });

    it("should throw error when reloadOverview fails", async () => {
      mockReloadOverview.mockRejectedValueOnce(ApiError);

      const { result } = renderHook(() => useDashboard());

      await expect(result.current.loadOverview()).rejects.toEqual(ApiError);

      expect(mockReloadOverview).toHaveBeenCalledWith(
        "/api/Dashboard/overview",
      );
    });
  });

  describe("getOverview", () => {
    it("should fetch overview with GET and updateState true", async () => {
      const mockOverview = {
        totalProducts: 30,
        totalCategories: 6,
        totalSuppliers: 10,
        lowStockCount: 2,
      };

      mockRequest.mockResolvedValueOnce(mockOverview);

      const { result } = renderHook(() => useDashboard());

      let data;
      await act(async () => {
        data = await result.current.getOverview();
      });

      expect(mockRequest).toHaveBeenCalledWith("/api/Dashboard/overview", {
        method: "GET",
        updateState: true,
      });
      expect(data).toEqual(mockOverview);
    });

    it("should throw API error when getOverview fails", async () => {
      mockRequest.mockRejectedValueOnce(ApiError);

      const { result } = renderHook(() => useDashboard());

      await expect(result.current.getOverview()).rejects.toEqual(ApiError);

      expect(mockRequest).toHaveBeenCalledWith("/api/Dashboard/overview", {
        method: "GET",
        updateState: true,
      });
    });

    it("should throw not found error when getOverview fails with 404", async () => {
      mockRequest.mockRejectedValueOnce(NotFoundError);

      const { result } = renderHook(() => useDashboard());

      await expect(result.current.getOverview()).rejects.toEqual(NotFoundError);

      expect(mockRequest).toHaveBeenCalledWith("/api/Dashboard/overview", {
        method: "GET",
        updateState: true,
      });
    });
  });
});
