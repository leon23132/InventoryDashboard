import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act } from "@testing-library/react";
import useCategories from "./useCategories";
import useApiResource from "./useApiResource";

vi.mock("./useApiResource", () => ({
  default: vi.fn(),
}));

const mockReloadCategories = vi.fn();
const mockRequest = vi.fn();
// const DeleteSuccessResponse = new Response("No Content", { status: 204 });
const NotFoundError = { status: 404, message: "Not Found" };
const ApiError = new Error("API Error");
const ValidationError = { status: 400, message: "Validation Error" };
const NetworkError = new Error("Network Error");
describe("useCategories", () => {
  beforeEach(() => {
    vi.clearAllMocks();

    useApiResource.mockReturnValue({
      data: [{ categoryId: 1, categoryName: "Hardware" }],
      loading: false,
      error: null,
      reload: mockReloadCategories,
      request: mockRequest,
      setData: vi.fn(),
    });
  });
  describe("Get Category By ID", () => {
    // Placeholder for the Get Category By ID test
    it("Get Category By ID", async () => {
      // Mock the API response for getting a category by ID
      const mockCategory = { categoryId: 3, categoryName: "Hardware" };
      // Set up the mock request to resolve with the mock category
      mockRequest.mockResolvedValueOnce(mockCategory);

      // Render the hook to access its functions
      const { result } = renderHook(() => useCategories());

      // Call the getCategoryById function with a specific category ID
      let data;

      await act(async () => {
        data = await result.current.getCategoryById(3);
      });

      // Assert that the request function was called with the correct URL and method
      expect(mockRequest).toHaveBeenCalledWith("/api/categories/3", {
        method: "GET",
      });
      // Assert that the data returned from getCategoryById matches the mock category
      expect(data).toEqual(mockCategory);
    });

    it("Get By Category ID - Not Found", async () => {
      mockRequest.mockRejectedValueOnce({
        status: NotFoundError.status,
        message: NotFoundError.message,
      });
      // Render the hook to access its functions
      const { result } = renderHook(() => useCategories());

      // Call the getCategoryById function with a non-existent category ID and expect it to reject with the NotFoundError
      await expect(result.current.getCategoryById(999)).rejects.toEqual({
        status: NotFoundError.status,
        message: NotFoundError.message,
      });
      // Assert that the request function was called with the correct URL and method for the non-existent category ID
      expect(mockRequest).toHaveBeenCalledWith("/api/categories/999", {
        method: "GET",
      });
    });

    it("Get By Category ID - API Error", async () => {
      // Mock the API response to reject with an API error
      mockRequest.mockRejectedValueOnce(ApiError);
      const { result } = renderHook(() => useCategories());

      // Call the getCategoryById function and expect it to reject with the API error
      await expect(result.current.getCategoryById(3)).rejects.toEqual(ApiError);
      // Assert that the request function was called with the correct URL and method
      expect(mockRequest).toHaveBeenCalledWith("/api/categories/3", {
        method: "GET",
      });
    });
  });

  describe("Load Categories with Filters", () => {
    it("Load Categories with Filters", async () => {
      const { result } = renderHook(() => useCategories());
      await act(async () => {
        await result.current.loadCategories({
          search: "Hardware",
          page: 1,
          pageSize: 10,
        });
      });
      expect(mockReloadCategories).toHaveBeenCalledWith(
        "/api/categories?search=Hardware&page=1&pageSize=10",
      );
    });

    it("Load Categories with Default Filters", async () => {
      const { result } = renderHook(() => useCategories());
      await act(async () => {
        await result.current.loadCategories();
      });
      expect(mockReloadCategories).toHaveBeenCalledWith(
        "/api/categories?page=1&pageSize=10",
      );
    });

    it("Load Categories - Not Found", async () => {
      mockReloadCategories.mockRejectedValueOnce({
        status: NotFoundError.status,
        message: NotFoundError.message,
      });
      const { result } = renderHook(() => useCategories());
      await expect(result.current.loadCategories()).rejects.toEqual({
        status: NotFoundError.status,
        message: NotFoundError.message,
      });
      expect(mockReloadCategories).toHaveBeenCalledWith(
        "/api/categories?page=1&pageSize=10",
      );
    });

    it("Load Categories - API Error", async () => {
      mockReloadCategories.mockRejectedValueOnce(ApiError);
      const { result } = renderHook(() => useCategories());
      await expect(result.current.loadCategories()).rejects.toEqual(ApiError);
      expect(mockReloadCategories).toHaveBeenCalledWith(
        "/api/categories?page=1&pageSize=10",
      );
    });

    it("Load Categories - With Search Only", async () => {
      const { result } = renderHook(() => useCategories());
      await act(async () => {
        await result.current.loadCategories({ search: "Hardware" });
      });
      expect(mockReloadCategories).toHaveBeenCalledWith(
        "/api/categories?search=Hardware&page=1&pageSize=10",
      );
    });
  });

  describe("Delete Category", () => {
    it("Delete Category - Success", async () => {
      mockRequest.mockResolvedValueOnce();
      mockReloadCategories.mockResolvedValueOnce();
      // Render the hook to access its functions
      const { result } = renderHook(() => useCategories());
      // Call the deleteCategory function with a specific category ID
      let success;
      await act(async () => {
        success = await result.current.deleteCategory(3);
      });

      expect(mockRequest).toHaveBeenCalledWith("/api/categories/3", {
        method: "DELETE",
      });
      expect(mockReloadCategories).toHaveBeenCalled();
      expect(success).toBe(true);
    });

    it("Delete Category - API Error", async () => {
      mockRequest.mockRejectedValueOnce(ApiError);
      const { result } = renderHook(() => useCategories());

      await expect(result.current.deleteCategory(3)).rejects.toEqual(ApiError);
      expect(mockRequest).toHaveBeenCalledWith("/api/categories/3", {
        method: "DELETE",
      });

      expect(mockReloadCategories).not.toHaveBeenCalled();
    });
    it("Delete Category - Not Found", async () => {
      mockRequest.mockRejectedValueOnce({
        status: NotFoundError.status,
        message: NotFoundError.message,
      });
      const { result } = renderHook(() => useCategories());
      await expect(result.current.deleteCategory(3)).rejects.toEqual({
        status: NotFoundError.status,
        message: NotFoundError.message,
      });
      expect(mockRequest).toHaveBeenCalledWith("/api/categories/3", {
        method: "DELETE",
      });
      expect(mockReloadCategories).not.toHaveBeenCalled();
    });
  });
  describe("Create Category", () => {
    it("Create Category", async () => {
      const payload = { name: "Hardware" };
      const mockResponse = { categoryId: 3, categoryName: "Hardware" };

      mockRequest.mockResolvedValueOnce(mockResponse);

      const { result } = renderHook(() => useCategories());
      let data;
      await act(async () => {
        data = await result.current.createCategory(payload);
      });

      expect(mockRequest).toHaveBeenCalledWith("/api/categories", {
        method: "POST",
        body: JSON.stringify(payload),
      });
      expect(data).toEqual(mockResponse);
    });
    it("Create Category - API Error", async () => {
      const payload = { name: "Hardware" };

      mockRequest.mockRejectedValueOnce(ApiError);

      const { result } = renderHook(() => useCategories());

      await expect(result.current.createCategory(payload)).rejects.toEqual(
        ApiError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/categories", {
        method: "POST",
        body: JSON.stringify(payload),
      });
    });
    it("Create Category - Validation Error", async () => {
      const payload = { name: "" }; // Invalid payload

      mockRequest.mockRejectedValueOnce(ValidationError);
      const { result } = renderHook(() => useCategories());
      await expect(result.current.createCategory(payload)).rejects.toEqual(
        ValidationError,
      );
      expect(mockRequest).toHaveBeenCalledWith("/api/categories", {
        method: "POST",
        body: JSON.stringify(payload),
      });
    });
    it("Create Category - Network Error", async () => {
      const payload = { name: "Hardware" };

      mockRequest.mockRejectedValueOnce(NetworkError);

      const { result } = renderHook(() => useCategories());

      await expect(result.current.createCategory(payload)).rejects.toEqual(
        NetworkError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/categories", {
        method: "POST",
        body: JSON.stringify(payload),
      });
    });
  });
  describe("Update Category", () => {
    it("Update Category - Success", async () => {
      const payload = { name: "Updated Hardware" };
      const updatedCategory = { categoryId: 3, name: "Updated Hardware" };

      mockRequest.mockResolvedValueOnce(updatedCategory);

      const { result } = renderHook(() => useCategories());

      let data;
      await act(async () => {
        data = await result.current.updateCategory(3, payload);
      });

      expect(mockRequest).toHaveBeenCalledWith("/api/categories/3", {
        method: "PUT",
        body: JSON.stringify(payload),
      });
      expect(data).toEqual(updatedCategory);
    });

    it("Update Category - Not Found", async () => {
      const payload = { name: "Updated Hardware" };

      mockRequest.mockRejectedValueOnce(NotFoundError);

      const { result } = renderHook(() => useCategories());

      await expect(result.current.updateCategory(999, payload)).rejects.toEqual(
        NotFoundError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/categories/999", {
        method: "PUT",
        body: JSON.stringify(payload),
      });
    });

    it("Update Category - Validation Error", async () => {
      const payload = { name: "" };

      mockRequest.mockRejectedValueOnce(ValidationError);

      const { result } = renderHook(() => useCategories());

      await expect(result.current.updateCategory(3, payload)).rejects.toEqual(
        ValidationError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/categories/3", {
        method: "PUT",
        body: JSON.stringify(payload),
      });
    });

    it("Update Category - API Error", async () => {
      const payload = { categoryName: "Updated Hardware" };

      mockRequest.mockRejectedValueOnce(ApiError);

      const { result } = renderHook(() => useCategories());

      await expect(result.current.updateCategory(3, payload)).rejects.toEqual(
        ApiError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/categories/3", {
        method: "PUT",
        body: JSON.stringify(payload),
      });
    });
  });
});
