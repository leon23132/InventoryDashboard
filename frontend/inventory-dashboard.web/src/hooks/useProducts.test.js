import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act } from "@testing-library/react";
import useProducts from "./useProducts";
import useApiResource from "./useApiResource";

vi.mock("./useApiResource", () => ({
  default: vi.fn(),
}));

const mockReload = vi.fn();
const mockRequest = vi.fn();
const mockSetProducts = vi.fn();

const NotFoundError = { status: 404, message: "Not Found" };
const ApiError = new Error("API Error");
const ValidationError = { status: 400, message: "Validation Error" };
const NetworkError = new Error("Network Error");

describe("useProducts", () => {
  beforeEach(() => {
    vi.clearAllMocks();

    useApiResource.mockReturnValue({
      data: [
        {
          productId: 1,
          productTitle: "Laptop",
          productDescription: "Business Laptop",
          categoryId: 1,
          supplierId: 1,
          price: 1499.9,
          quantityInStock: 5,
          location: "Regal A1",
        },
      ],
      loading: false,
      error: null,
      reload: mockReload,
      request: mockRequest,
      setData: mockSetProducts,
    });
  });

  describe("Get Product By ID", () => {
    it("Get Product By ID - Success", async () => {
      const mockProduct = {
        productId: 3,
        productTitle: "Monitor",
        productDescription: "27 Zoll Monitor",
        categoryId: 2,
        supplierId: 4,
        price: 299.9,
        quantityInStock: 12,
        location: "Regal B2",
      };

      mockRequest.mockResolvedValueOnce(mockProduct);

      const { result } = renderHook(() => useProducts());

      let data;
      await act(async () => {
        data = await result.current.getProductById(3);
      });

      expect(mockRequest).toHaveBeenCalledWith("/api/products/3", {
        method: "GET",
      });
      expect(data).toEqual(mockProduct);
    });

    it("Get Product By ID - Not Found", async () => {
      mockRequest.mockRejectedValueOnce(NotFoundError);

      const { result } = renderHook(() => useProducts());

      await expect(result.current.getProductById(999)).rejects.toEqual(
        NotFoundError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/products/999", {
        method: "GET",
      });
    });

    it("Get Product By ID - API Error", async () => {
      mockRequest.mockRejectedValueOnce(ApiError);

      const { result } = renderHook(() => useProducts());

      await expect(result.current.getProductById(3)).rejects.toEqual(ApiError);

      expect(mockRequest).toHaveBeenCalledWith("/api/products/3", {
        method: "GET",
      });
    });
  });

  describe("Load Products with Filters", () => {
    it("Load Products with Filters", async () => {
      const { result } = renderHook(() => useProducts());

      await act(async () => {
        result.current.loadProducts({
          search: "Laptop",
          categoryId: 1,
          supplierId: 2,
          page: 1,
          pageSize: 10,
        });
      });

      expect(mockReload).toHaveBeenCalledWith(
        "/api/products?search=Laptop&categoryId=1&supplierId=2&page=1&pageSize=10",
      );
    });

    it("Load Products with Default Filters", async () => {
      const { result } = renderHook(() => useProducts());

      await act(async () => {
        result.current.loadProducts();
      });

      expect(mockReload).toHaveBeenCalledWith(
        "/api/products?page=1&pageSize=10",
      );
    });

    it("Load Products - With Search Only", async () => {
      const { result } = renderHook(() => useProducts());

      await act(async () => {
        result.current.loadProducts({ search: "Laptop" });
      });

      expect(mockReload).toHaveBeenCalledWith(
        "/api/products?search=Laptop&page=1&pageSize=10",
      );
    });

    it("Load Products - With Category Only", async () => {
      const { result } = renderHook(() => useProducts());

      await act(async () => {
        result.current.loadProducts({ categoryId: 1 });
      });

      expect(mockReload).toHaveBeenCalledWith(
        "/api/products?categoryId=1&page=1&pageSize=10",
      );
    });

    it("Load Products - With Supplier Only", async () => {
      const { result } = renderHook(() => useProducts());

      await act(async () => {
        result.current.loadProducts({ supplierId: 2 });
      });

      expect(mockReload).toHaveBeenCalledWith(
        "/api/products?supplierId=2&page=1&pageSize=10",
      );
    });

    it("Load Products - Invalid Page Uses Default", async () => {
      const { result } = renderHook(() => useProducts());

      await act(async () => {
        result.current.loadProducts({ page: 0, pageSize: 10 });
      });

      expect(mockReload).toHaveBeenCalledWith(
        "/api/products?page=1&pageSize=10",
      );
    });

    it("Load Products - Invalid PageSize Uses Default", async () => {
      const { result } = renderHook(() => useProducts());

      await act(async () => {
        result.current.loadProducts({ page: 1, pageSize: 0 });
      });

      expect(mockReload).toHaveBeenCalledWith(
        "/api/products?page=1&pageSize=10",
      );
    });

    it("Load Products - PageSize Above 100 Uses Max 100", async () => {
      const { result } = renderHook(() => useProducts());

      await act(async () => {
        result.current.loadProducts({ page: 1, pageSize: 200 });
      });

      expect(mockReload).toHaveBeenCalledWith(
        "/api/products?page=1&pageSize=100",
      );
    });

    it("Load Products - Trims Search", async () => {
      const { result } = renderHook(() => useProducts());

      await act(async () => {
        result.current.loadProducts({ search: "  Laptop  " });
      });

      expect(mockReload).toHaveBeenCalledWith(
        "/api/products?search=Laptop&page=1&pageSize=10",
      );
    });
  });

  describe("Delete Product", () => {
    it("Delete Product - Success", async () => {
      mockRequest.mockResolvedValueOnce(null);

      const { result } = renderHook(() => useProducts());

      let success;
      await act(async () => {
        success = await result.current.deleteProduct(3);
      });

      expect(mockRequest).toHaveBeenCalledWith("/api/products/3", {
        method: "DELETE",
      });
      expect(mockSetProducts).not.toHaveBeenCalled();
      expect(success).toBe(true);
    });

    it("Delete Product - Calls correct endpoint", async () => {
      mockRequest.mockResolvedValueOnce(null);

      const { result } = renderHook(() => useProducts());

      await act(async () => {
        await result.current.deleteProduct(3);
      });

      expect(mockRequest).toHaveBeenCalledTimes(1);
      expect(mockRequest).toHaveBeenCalledWith("/api/products/3", {
        method: "DELETE",
      });
      expect(mockSetProducts).not.toHaveBeenCalled();
    });

    it("Delete Product - API Error", async () => {
      mockRequest.mockRejectedValueOnce(ApiError);

      const { result } = renderHook(() => useProducts());

      await expect(result.current.deleteProduct(3)).rejects.toThrow(
        "API Error",
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/products/3", {
        method: "DELETE",
      });
      expect(mockSetProducts).not.toHaveBeenCalled();
    });

    it("Delete Product - Not Found", async () => {
      mockRequest.mockRejectedValueOnce(NotFoundError);

      const { result } = renderHook(() => useProducts());

      await expect(result.current.deleteProduct(999)).rejects.toThrow(
        "Not Found",
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/products/999", {
        method: "DELETE",
      });
      expect(mockSetProducts).not.toHaveBeenCalled();
    });
  });

  describe("Create Product", () => {
    it("Create Product - Success", async () => {
      const payload = {
        productTitle: "Laptop",
        productDescription: "Business Laptop",
        categoryId: 1,
        supplierId: 2,
        price: 1499.9,
        quantityInStock: 5,
        location: "Regal A1",
      };

      const mockResponse = {
        productId: 3,
        ...payload,
      };

      mockRequest.mockResolvedValueOnce(mockResponse);

      const { result } = renderHook(() => useProducts());

      let data;
      await act(async () => {
        data = await result.current.createProduct(payload);
      });

      expect(mockRequest).toHaveBeenCalledWith("/api/products", {
        method: "POST",
        body: JSON.stringify(payload),
      });
      expect(data).toEqual(mockResponse);
    });

    it("Create Product - API Error", async () => {
      const payload = {
        productTitle: "Laptop",
        productDescription: "Business Laptop",
        categoryId: 1,
        supplierId: 2,
        price: 1499.9,
        quantityInStock: 5,
        location: "Regal A1",
      };

      mockRequest.mockRejectedValueOnce(ApiError);

      const { result } = renderHook(() => useProducts());

      await expect(result.current.createProduct(payload)).rejects.toEqual(
        ApiError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/products", {
        method: "POST",
        body: JSON.stringify(payload),
      });
    });

    it("Create Product - Validation Error", async () => {
      const payload = {
        productTitle: "",
        productDescription: "Business Laptop",
        categoryId: 1,
        supplierId: 2,
        price: 1499.9,
        quantityInStock: 5,
        location: "Regal A1",
      };

      mockRequest.mockRejectedValueOnce(ValidationError);

      const { result } = renderHook(() => useProducts());

      await expect(result.current.createProduct(payload)).rejects.toEqual(
        ValidationError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/products", {
        method: "POST",
        body: JSON.stringify(payload),
      });
    });

    it("Create Product - Network Error", async () => {
      const payload = {
        productTitle: "Laptop",
        productDescription: "Business Laptop",
        categoryId: 1,
        supplierId: 2,
        price: 1499.9,
        quantityInStock: 5,
        location: "Regal A1",
      };

      mockRequest.mockRejectedValueOnce(NetworkError);

      const { result } = renderHook(() => useProducts());

      await expect(result.current.createProduct(payload)).rejects.toEqual(
        NetworkError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/products", {
        method: "POST",
        body: JSON.stringify(payload),
      });
    });
  });

  describe("Update Product", () => {
    it("Update Product - Success", async () => {
      const payload = {
        productTitle: "Updated Laptop",
        productDescription: "Updated Description",
        categoryId: 1,
        supplierId: 2,
        price: 1999.0,
        quantityInStock: 12,
        location: "Regal C3",
      };

      const updatedProduct = {
        productId: 3,
        ...payload,
      };

      mockRequest.mockResolvedValueOnce(updatedProduct);

      const { result } = renderHook(() => useProducts());

      let data;
      await act(async () => {
        data = await result.current.updateProduct(3, payload);
      });

      expect(mockRequest).toHaveBeenCalledWith("/api/products/3", {
        method: "PUT",
        body: JSON.stringify(payload),
      });
      expect(data).toEqual(updatedProduct);
    });

    it("Update Product - Not Found", async () => {
      const payload = {
        productTitle: "Updated Laptop",
        productDescription: "Updated Description",
        categoryId: 1,
        supplierId: 2,
        price: 1999.0,
        quantityInStock: 12,
        location: "Regal C3",
      };

      mockRequest.mockRejectedValueOnce(NotFoundError);

      const { result } = renderHook(() => useProducts());

      await expect(result.current.updateProduct(999, payload)).rejects.toEqual(
        NotFoundError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/products/999", {
        method: "PUT",
        body: JSON.stringify(payload),
      });
    });

    it("Update Product - Validation Error", async () => {
      const payload = {
        productTitle: "",
        productDescription: "Updated Description",
        categoryId: 1,
        supplierId: 2,
        price: 1999.0,
        quantityInStock: 12,
        location: "Regal C3",
      };

      mockRequest.mockRejectedValueOnce(ValidationError);

      const { result } = renderHook(() => useProducts());

      await expect(result.current.updateProduct(3, payload)).rejects.toEqual(
        ValidationError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/products/3", {
        method: "PUT",
        body: JSON.stringify(payload),
      });
    });

    it("Update Product - API Error", async () => {
      const payload = {
        productTitle: "Updated Laptop",
        productDescription: "Updated Description",
        categoryId: 1,
        supplierId: 2,
        price: 1999.0,
        quantityInStock: 12,
        location: "Regal C3",
      };

      mockRequest.mockRejectedValueOnce(ApiError);

      const { result } = renderHook(() => useProducts());

      await expect(result.current.updateProduct(3, payload)).rejects.toEqual(
        ApiError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/products/3", {
        method: "PUT",
        body: JSON.stringify(payload),
      });
    });
  });
});
