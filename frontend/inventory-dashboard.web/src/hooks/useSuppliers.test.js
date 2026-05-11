import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act } from "@testing-library/react";
import useSuppliers from "./useSuppliers";
import useApiResource from "./useApiResource";

vi.mock("./useApiResource", () => ({
  default: vi.fn(),
}));

const mockReloadSuppliers = vi.fn();
const mockRequest = vi.fn();

const NotFoundError = { status: 404, message: "Not Found" };
const ApiError = new Error("API Error");
const ValidationError = { status: 400, message: "Validation Error" };
const NetworkError = new Error("Network Error");

describe("useSuppliers", () => {
  beforeEach(() => {
    vi.clearAllMocks();

    useApiResource.mockReturnValue({
      data: [
        {
          supplierId: 1,
          companyName: "Tech AG",
          contactPerson: "Max Muster",
          email: "max@techag.ch",
          phoneNumber: "+41 44 123 45 67",
          website: "https://techag.ch",
          billingAddress: {
            addressId: 1,
            streetAddress: "Bahnhofstrasse 1",
            city: "Zürich",
            postalCode: "8001",
            country: "Switzerland",
          },
          shippingAddress: {
            addressId: 2,
            streetAddress: "Lagerstrasse 10",
            city: "Zürich",
            postalCode: "8004",
            country: "Switzerland",
          },
        },
      ],
      loading: false,
      error: null,
      reload: mockReloadSuppliers,
      request: mockRequest,
      setData: vi.fn(),
    });
  });

  describe("Get Supplier By ID", () => {
    it("Get Supplier By ID", async () => {
      const mockSupplier = {
        supplierId: 3,
        companyName: "Hardware AG",
        contactPerson: "Hans Meier",
        email: "hans@hardwareag.ch",
        phoneNumber: "+41 52 123 45 67",
        website: "https://hardwareag.ch",
        billingAddress: {
          addressId: 3,
          streetAddress: "Industriestrasse 5",
          city: "Winterthur",
          postalCode: "8400",
          country: "Switzerland",
        },
        shippingAddress: {
          addressId: 4,
          streetAddress: "Lagerweg 8",
          city: "Winterthur",
          postalCode: "8404",
          country: "Switzerland",
        },
      };

      mockRequest.mockResolvedValueOnce(mockSupplier);

      const { result } = renderHook(() => useSuppliers());

      let data;
      await act(async () => {
        data = await result.current.getSupplierById(3);
      });

      expect(mockRequest).toHaveBeenCalledWith("/api/suppliers/3", {
        method: "GET",
      });
      expect(data).toEqual(mockSupplier);
    });

    it("Get By Supplier ID - Not Found", async () => {
      mockRequest.mockRejectedValueOnce(NotFoundError);

      const { result } = renderHook(() => useSuppliers());

      await expect(result.current.getSupplierById(999)).rejects.toEqual(
        NotFoundError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/suppliers/999", {
        method: "GET",
      });
    });

    it("Get By Supplier ID - API Error", async () => {
      mockRequest.mockRejectedValueOnce(ApiError);

      const { result } = renderHook(() => useSuppliers());

      await expect(result.current.getSupplierById(3)).rejects.toEqual(ApiError);

      expect(mockRequest).toHaveBeenCalledWith("/api/suppliers/3", {
        method: "GET",
      });
    });
  });

  describe("Load Suppliers with Filters", () => {
    it("Load Suppliers with Filters", async () => {
      const { result } = renderHook(() => useSuppliers());

      await act(async () => {
        await result.current.loadSuppliers({
          search: "Tech",
          contactPerson: "Max",
          city: "Zürich",
          page: 1,
          pageSize: 10,
        });
      });

      expect(mockReloadSuppliers).toHaveBeenCalledWith(
        "/api/suppliers?search=Tech&contactPerson=Max&city=Z%C3%BCrich&page=1&pageSize=10",
      );
    });

    it("Load Suppliers with Default Filters", async () => {
      const { result } = renderHook(() => useSuppliers());

      await act(async () => {
        await result.current.loadSuppliers();
      });

      expect(mockReloadSuppliers).toHaveBeenCalledWith(
        "/api/suppliers?page=1&pageSize=10",
      );
    });

    it("Load Suppliers - Not Found", async () => {
      mockReloadSuppliers.mockRejectedValueOnce(NotFoundError);

      const { result } = renderHook(() => useSuppliers());

      await expect(result.current.loadSuppliers()).rejects.toEqual(
        NotFoundError,
      );

      expect(mockReloadSuppliers).toHaveBeenCalledWith(
        "/api/suppliers?page=1&pageSize=10",
      );
    });

    it("Load Suppliers - API Error", async () => {
      mockReloadSuppliers.mockRejectedValueOnce(ApiError);

      const { result } = renderHook(() => useSuppliers());

      await expect(result.current.loadSuppliers()).rejects.toEqual(ApiError);

      expect(mockReloadSuppliers).toHaveBeenCalledWith(
        "/api/suppliers?page=1&pageSize=10",
      );
    });

    it("Load Suppliers - With Search Only", async () => {
      const { result } = renderHook(() => useSuppliers());

      await act(async () => {
        await result.current.loadSuppliers({ search: "Tech" });
      });

      expect(mockReloadSuppliers).toHaveBeenCalledWith(
        "/api/suppliers?search=Tech&page=1&pageSize=10",
      );
    });

    it("Load Suppliers - With Contact Person Only", async () => {
      const { result } = renderHook(() => useSuppliers());

      await act(async () => {
        await result.current.loadSuppliers({ contactPerson: "Max" });
      });

      expect(mockReloadSuppliers).toHaveBeenCalledWith(
        "/api/suppliers?contactPerson=Max&page=1&pageSize=10",
      );
    });

    it("Load Suppliers - With City Only", async () => {
      const { result } = renderHook(() => useSuppliers());

      await act(async () => {
        await result.current.loadSuppliers({ city: "Zürich" });
      });

      expect(mockReloadSuppliers).toHaveBeenCalledWith(
        "/api/suppliers?city=Z%C3%BCrich&page=1&pageSize=10",
      );
    });
  });

  describe("Delete Supplier", () => {
    it("Delete Supplier - Success", async () => {
      mockRequest.mockResolvedValueOnce();
      mockReloadSuppliers.mockResolvedValueOnce();

      const { result } = renderHook(() => useSuppliers());

      let success;
      await act(async () => {
        success = await result.current.deleteSupplier(3);
      });

      expect(mockRequest).toHaveBeenCalledWith("/api/suppliers/3", {
        method: "DELETE",
      });
      expect(mockReloadSuppliers).toHaveBeenCalled();
      expect(success).toBe(true);
    });

    it("Delete Supplier - API Error", async () => {
      mockRequest.mockRejectedValueOnce(ApiError);

      const { result } = renderHook(() => useSuppliers());

      await expect(result.current.deleteSupplier(3)).rejects.toEqual(ApiError);

      expect(mockRequest).toHaveBeenCalledWith("/api/suppliers/3", {
        method: "DELETE",
      });
      expect(mockReloadSuppliers).not.toHaveBeenCalled();
    });

    it("Delete Supplier - Not Found", async () => {
      mockRequest.mockRejectedValueOnce(NotFoundError);

      const { result } = renderHook(() => useSuppliers());

      await expect(result.current.deleteSupplier(3)).rejects.toEqual(
        NotFoundError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/suppliers/3", {
        method: "DELETE",
      });
      expect(mockReloadSuppliers).not.toHaveBeenCalled();
    });
  });

  describe("Create Supplier", () => {
    it("Create Supplier - Success", async () => {
      const payload = {
        companyName: "Hardware AG",
        contactPerson: "Hans Meier",
        email: "hans@hardwareag.ch",
        phoneNumber: "+41 52 123 45 67",
        website: "https://hardwareag.ch",
        billingAddress: {
          streetAddress: "Industriestrasse 5",
          city: "Winterthur",
          postalCode: "8400",
          country: "Switzerland",
        },
        shippingAddress: {
          streetAddress: "Lagerweg 8",
          city: "Winterthur",
          postalCode: "8404",
          country: "Switzerland",
        },
      };

      const mockResponse = {
        supplierId: 3,
        companyName: "Hardware AG",
        contactPerson: "Hans Meier",
        email: "hans@hardwareag.ch",
        phoneNumber: "+41 52 123 45 67",
        website: "https://hardwareag.ch",
        billingAddress: {
          addressId: 3,
          streetAddress: "Industriestrasse 5",
          city: "Winterthur",
          postalCode: "8400",
          country: "Switzerland",
        },
        shippingAddress: {
          addressId: 4,
          streetAddress: "Lagerweg 8",
          city: "Winterthur",
          postalCode: "8404",
          country: "Switzerland",
        },
      };

      mockRequest.mockResolvedValueOnce(mockResponse);

      const { result } = renderHook(() => useSuppliers());

      let data;
      await act(async () => {
        data = await result.current.createSupplier(payload);
      });

      expect(mockRequest).toHaveBeenCalledWith("/api/suppliers", {
        method: "POST",
        body: JSON.stringify(payload),
      });
      expect(data).toEqual(mockResponse);
    });

    it("Create Supplier - API Error", async () => {
      const payload = {
        companyName: "Hardware AG",
        contactPerson: "Hans Meier",
        email: "hans@hardwareag.ch",
        phoneNumber: "+41 52 123 45 67",
        website: "https://hardwareag.ch",
        billingAddress: {
          streetAddress: "Industriestrasse 5",
          city: "Winterthur",
          postalCode: "8400",
          country: "Switzerland",
        },
        shippingAddress: {
          streetAddress: "Lagerweg 8",
          city: "Winterthur",
          postalCode: "8404",
          country: "Switzerland",
        },
      };

      mockRequest.mockRejectedValueOnce(ApiError);

      const { result } = renderHook(() => useSuppliers());

      await expect(result.current.createSupplier(payload)).rejects.toEqual(
        ApiError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/suppliers", {
        method: "POST",
        body: JSON.stringify(payload),
      });
    });

    it("Create Supplier - Validation Error", async () => {
      const payload = {
        companyName: "",
        contactPerson: "Hans Meier",
        email: "invalid-mail",
        phoneNumber: "+41 52 123 45 67",
        website: "https://hardwareag.ch",
        billingAddress: {
          streetAddress: "",
          city: "Winterthur",
          postalCode: "8400",
          country: "Switzerland",
        },
      };

      mockRequest.mockRejectedValueOnce(ValidationError);

      const { result } = renderHook(() => useSuppliers());

      await expect(result.current.createSupplier(payload)).rejects.toEqual(
        ValidationError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/suppliers", {
        method: "POST",
        body: JSON.stringify(payload),
      });
    });

    it("Create Supplier - Network Error", async () => {
      const payload = {
        companyName: "Hardware AG",
        contactPerson: "Hans Meier",
        email: "hans@hardwareag.ch",
        phoneNumber: "+41 52 123 45 67",
        website: "https://hardwareag.ch",
        billingAddress: {
          streetAddress: "Industriestrasse 5",
          city: "Winterthur",
          postalCode: "8400",
          country: "Switzerland",
        },
        shippingAddress: {
          streetAddress: "Lagerweg 8",
          city: "Winterthur",
          postalCode: "8404",
          country: "Switzerland",
        },
      };

      mockRequest.mockRejectedValueOnce(NetworkError);

      const { result } = renderHook(() => useSuppliers());

      await expect(result.current.createSupplier(payload)).rejects.toEqual(
        NetworkError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/suppliers", {
        method: "POST",
        body: JSON.stringify(payload),
      });
    });
  });

  describe("Update Supplier", () => {
    it("Update Supplier - Success", async () => {
      const payload = {
        companyName: "Updated Hardware AG",
        contactPerson: "Peter Keller",
        email: "peter@hardwareag.ch",
        phoneNumber: "+41 31 123 45 67",
        website: "https://hardwareag.ch",
        billingAddress: {
          streetAddress: "Marktgasse 12",
          city: "Bern",
          postalCode: "3000",
          country: "Switzerland",
        },
        shippingAddress: {
          streetAddress: "Lagerweg 20",
          city: "Bern",
          postalCode: "3011",
          country: "Switzerland",
        },
      };

      const updatedSupplier = {
        supplierId: 3,
        companyName: "Updated Hardware AG",
        contactPerson: "Peter Keller",
        email: "peter@hardwareag.ch",
        phoneNumber: "+41 31 123 45 67",
        website: "https://hardwareag.ch",
        billingAddress: {
          addressId: 5,
          streetAddress: "Marktgasse 12",
          city: "Bern",
          postalCode: "3000",
          country: "Switzerland",
        },
        shippingAddress: {
          addressId: 6,
          streetAddress: "Lagerweg 20",
          city: "Bern",
          postalCode: "3011",
          country: "Switzerland",
        },
      };

      mockRequest.mockResolvedValueOnce(updatedSupplier);

      const { result } = renderHook(() => useSuppliers());

      let data;
      await act(async () => {
        data = await result.current.updateSupplier(3, payload);
      });

      expect(mockRequest).toHaveBeenCalledWith("/api/suppliers/3", {
        method: "PUT",
        body: JSON.stringify(payload),
      });
      expect(data).toEqual(updatedSupplier);
    });

    it("Update Supplier - Not Found", async () => {
      const payload = {
        companyName: "Updated Hardware AG",
        contactPerson: "Peter Keller",
        email: "peter@hardwareag.ch",
        phoneNumber: "+41 31 123 45 67",
        website: "https://hardwareag.ch",
        billingAddress: {
          streetAddress: "Marktgasse 12",
          city: "Bern",
          postalCode: "3000",
          country: "Switzerland",
        },
      };

      mockRequest.mockRejectedValueOnce(NotFoundError);

      const { result } = renderHook(() => useSuppliers());

      await expect(result.current.updateSupplier(999, payload)).rejects.toEqual(
        NotFoundError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/suppliers/999", {
        method: "PUT",
        body: JSON.stringify(payload),
      });
    });

    it("Update Supplier - Validation Error", async () => {
      const payload = {
        companyName: "",
        contactPerson: "Peter Keller",
        email: "invalid-mail",
        phoneNumber: "+41 31 123 45 67",
        website: "https://hardwareag.ch",
        billingAddress: {
          streetAddress: "",
          city: "Bern",
          postalCode: "3000",
          country: "Switzerland",
        },
      };

      mockRequest.mockRejectedValueOnce(ValidationError);

      const { result } = renderHook(() => useSuppliers());

      await expect(result.current.updateSupplier(3, payload)).rejects.toEqual(
        ValidationError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/suppliers/3", {
        method: "PUT",
        body: JSON.stringify(payload),
      });
    });

    it("Update Supplier - API Error", async () => {
      const payload = {
        companyName: "Updated Hardware AG",
        contactPerson: "Peter Keller",
        email: "peter@hardwareag.ch",
        phoneNumber: "+41 31 123 45 67",
        website: "https://hardwareag.ch",
        billingAddress: {
          streetAddress: "Marktgasse 12",
          city: "Bern",
          postalCode: "3000",
          country: "Switzerland",
        },
        shippingAddress: {
          streetAddress: "Lagerweg 20",
          city: "Bern",
          postalCode: "3011",
          country: "Switzerland",
        },
      };

      mockRequest.mockRejectedValueOnce(ApiError);

      const { result } = renderHook(() => useSuppliers());

      await expect(result.current.updateSupplier(3, payload)).rejects.toEqual(
        ApiError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/suppliers/3", {
        method: "PUT",
        body: JSON.stringify(payload),
      });
    });
  });
});