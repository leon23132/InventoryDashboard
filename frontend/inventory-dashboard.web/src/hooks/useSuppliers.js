import useApiResource from "./useApiResource";
import { useCallback } from "react";

function buildSuppliersURL(search, contactPerson, city, page, pageSize) {
  // Construct query parameters based on provided filters
  const params = new URLSearchParams();

  // Append query parameters if they are provided
  if (search?.trim()) params.set("search", search.trim());
  if (contactPerson?.trim()) params.set("contactPerson", contactPerson.trim());
  if (city?.trim()) params.set("city", city.trim());

  params.set("page", page > 0 ? page : 1); // Default to page 1 if invalid
  params.set("pageSize", pageSize > 0 ? Math.min(pageSize, 100) : 10); // Default to 10, max 100

  // Construct the final URL with query string
  const qs = params.toString();
  return `/api/suppliers${qs ? `?${qs}` : ""}`;
}

export default function useSuppliers() {
  const {
    data: suppliers,
    loading: loadingSuppliers,
    error: suppliersError,
    reload: reloadSuppliers,
    request,
  } = useApiResource("/api/suppliers", { autoLoad: false });

  // Function to load suppliers with optional filters
  const loadSuppliers = useCallback(
    ({ search, contactPerson, city, page = 1, pageSize = 10 } = {}) => {
      // Build the URL with the provided filters
      const path = buildSuppliersURL(
        search,
        contactPerson,
        city,
        page,
        pageSize,
      );
      // Promise to reload suppliers from the constructed URL
      return reloadSuppliers(path);
    },
    [reloadSuppliers],
  );

  // Function to delete a supplier
  const deleteSupplier = useCallback(
    async (supplierId) => {
      // Send DELETE request to the API
      await request(`/api/suppliers/${supplierId}`, { method: "DELETE" });
      // Refresh the suppliers list after deletion
      reloadSuppliers();
      return true;
    },
    [request, reloadSuppliers],
  );

  const getSupplierById = useCallback(
    async (supplierId) => {
      // Find and return the supplier with the matching ID
      const data = await request(`/api/suppliers/${supplierId}`, {
        method: "GET",
      });
      return data;
    },
    [request],
  );

  // Function to create a new supplier
  const createSupplier = useCallback(
    async (payload) => {
      return await request(`/api/suppliers`, {
        method: "POST",
        body: JSON.stringify(payload),
      });
    },
    [request],
  );

  const updateSupplier = useCallback(
    async (supplierId, payload) => {
      return await request(`/api/suppliers/${supplierId}`, {
        method: "PUT",
        body: JSON.stringify(payload),
      });
    },
    [request],
  );

  return {
    suppliers: suppliers,
    loadingSuppliers: loadingSuppliers,
    suppliersError: suppliersError,
    reloadSuppliers: reloadSuppliers,
    loadSuppliers: loadSuppliers,
    deleteSupplier: deleteSupplier,
    getSupplierById: getSupplierById,
    createSupplier: createSupplier,
    updateSupplier: updateSupplier,
  };
}
