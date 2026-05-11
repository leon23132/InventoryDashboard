import { useCallback } from "react";
import useApiResource from "./useApiResource";

function buildProductsURL({ search, categoryId, supplierId, page, pageSize }) {
  // Create a new URL object for the products endpoint
  const params = new URLSearchParams();

  // Append query parameters based on provided filters
  if (search?.trim()) params.set("search", search.trim());
  if (categoryId) params.set("categoryId", categoryId);
  if (supplierId) params.set("supplierId", supplierId);

  params.set("page", page > 0 ? page : 1); // Default to page 1 if invalid
  params.set("pageSize", pageSize > 0 ? Math.min(pageSize, 100) : 10); // Default to 10, max 100

  // Construct the final URL with query string
  const qs = params.toString();
  return `/api/products${qs ? `?${qs}` : ""}`;
}

// Custom hook to manage products data and operations
export default function useProducts() {
  // Utilize the generic API resource hook to fetch products
  const {
    data: products,
    loading: loadingProducts,
    error: productsError,
    reload: reloadProducts,
    request,
  } = useApiResource("/api/products", { autoLoad: false });

  // Function to load products with optional filters
  const loadProducts = useCallback(
    ({
      search = "",
      categoryId = "",
      supplierId = "",
      page = 1,
      pageSize = 10,
    } = {}) => {
      const path = buildProductsURL({
        search,
        categoryId,
        supplierId,
        page,
        pageSize,
      });
      // Reload products from the constructed URL
      reloadProducts(path);
    },
    [reloadProducts],
  );

  // Function to delete a product by its ID
  const deleteProduct = useCallback(
    async (productId) => {
      // Send DELETE request to the API
      await request(`/api/products/${productId}`, { method: "DELETE" });
      // Update local state to remove the deleted product
      reloadProducts(); // Alternatively, you could optimistically update the state here
      return true;
    },
    [request, reloadProducts],
  );

  // Function to get a single product by its ID
  const getProductById = useCallback(
    async (productId) => {
      // Fetch a single product by its ID
      const data = await request(`/api/products/${productId}`, {
        method: "GET",
      });
      return data;
    },
    [request],
  );

  // Function to create a new product
  const createProduct = useCallback(
    (payload) =>
      request("/api/products", {
        method: "POST",
        body: JSON.stringify(payload),
      }),
    [request],
  );

  const updateProduct = useCallback(
    (Id, payload) =>
      request(`/api/products/${Id}`, {
        method: "PUT",
        body: JSON.stringify(payload),
      }),
    [request],
  );

  return {
    products,
    loadingProducts,
    productsError,
    loadProducts,
    deleteProduct,
    getProductById,
    createProduct,
    updateProduct,
  };
}
