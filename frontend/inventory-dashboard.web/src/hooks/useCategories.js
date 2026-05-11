import useApiResource from "./useApiResource";
import { useCallback } from "react";

function buildCategoriesURL({ search, page, pageSize }) {
  // Construct query parameters based on provided filters
  const params = new URLSearchParams();
  // Append query parameters if they are provided
  if (search?.trim()) params.set("search", search.trim());

  params.set("page", page > 0 ? page : 1); // Default to page 1 if invalid
  params.set("pageSize", pageSize > 0 ? Math.min(pageSize, 100) : 10); // Default to 10, max 100

  // Construct the final URL with query string
  const qs = params.toString();
  return `/api/categories${qs ? `?${qs}` : ""}`;
}

export default function useCategories() {
  // Utilize the generic API resource hook to fetch categories
  const {
    data: categories,
    loading: loadingCategories,
    error: categoriesError,
    reload: reloadCategories,
    request,
  } = useApiResource("/api/categories", { autoLoad: false });

  // Function to load categories with optional filters
  const loadCategories = useCallback(
    ({ search, page = 1, pageSize = 10 } = {}) => {
      // Build the URL with the provided filters
      const path = buildCategoriesURL({ search, page, pageSize });
      // Promise to reload categories from the constructed URL
      return reloadCategories(path);
    },
    [reloadCategories],
  );

  const deleteCategory = useCallback(
    async (categoryId) => {
      try {
        // Send DELETE request to the API
        await request(`/api/categories/${categoryId}`, { method: "DELETE" });
        // Refresh the categories list after deletion
        reloadCategories();
        return true;
      } catch (error) {
        throw error;
        // Proceed if the user confirms
      }
    },
    [request, reloadCategories],
  );

  // Function to get a category by its ID
  const getCategoryById = useCallback(
    async (categoryId) => {
      // Find and return the category with the matching ID
      const data = await request(`/api/categories/${categoryId}`, {
        method: "GET",
      });
      return data;
    },
    [request],
  );

  // Function to create a new category
  const createCategory = useCallback(
    async (payload) => {
      return await request(`/api/categories`, {
        method: "POST",
        body: JSON.stringify(payload),
      });
    },
    [request],
  );

  const updateCategory = useCallback(
    async (categoryId, payload) => {
      return await request(`/api/categories/${categoryId}`, {
        method: "PUT",
        body: JSON.stringify(payload),
      });
    },
    [request],
  );

  // Return categories data along with loading and error states
  return {
    categories: categories,
    loadingCategories: loadingCategories,
    categoriesError: categoriesError,
    reloadCategories: reloadCategories,
    deleteCategory: deleteCategory,
    getCategoryById: getCategoryById,
    createCategory: createCategory,
    updateCategory: updateCategory,
    loadCategories: loadCategories,
  };
}
