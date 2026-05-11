import { useState, useEffect, useCallback } from "react";
import { API_BASE_URL } from "../config/config";

export default function useApiResource(
  url,
  { autoLoad = true, abortPrevious = true } = {},
) {
  // State hooks for data, loading status, and error handling
  const [data, setData] = useState([]);
  const [error, setError] = useState(null);
  // Track the number of pending requests
  const [pendingCount, setPendingCount] = useState(0);
  // Derive loading state from pending requests
  const loading = pendingCount > 0;

  const request = useCallback(
    async (path, options = {}) => {
      // Determine the URL to fetch from
      const fetchUrl = path ?? url;

      const { updateState, ...fetchOptions } = options;

      const method = (fetchOptions.method ?? "GET").toUpperCase();
      const shouldUpdateState = updateState ?? method === "GET";

      try {
        //setLoading(true);
        setPendingCount((count) => count + 1);
        setError(null);

        // Make the API request
        const response = await fetch(`${API_BASE_URL}${fetchUrl}`, {
          headers: {
            "Content-Type": "application/json",
          },
          ...fetchOptions,
        });

        // Check if the response is successful
        if (!response.ok)
          throw new Error(
            `${method} ${fetchUrl} failed with status ${response.status}`,
          );

        // DELETE (204 No Content) responses have no body
        if (response.status === 204) return null;

        //Content type check (not used currently)
        const contentype = response.headers.get("Content-Type") || "";
        //
        const text = await response.text();

        // Parse JSON responses, otherwise return text
        const data = contentype.includes("application/json")
          ? JSON.parse(text)
          : text;

        // Update state if required
        if (shouldUpdateState) {
          setData(data);
        }
        return data;
      } catch (error) {
        // Handle errors by setting the error state
        setError(error);
        throw error;
      } finally {
        // Remove the controller from the set
        setPendingCount((count) => count - 1);
      }
    },
    [url, abortPrevious],
  );

  const reload = useCallback(
    (overridePath) => request(overridePath ?? url, { updateState: true }),
    [request, url],
  );

  useEffect(() => {
    if (autoLoad) reload().catch(() => {});
  }, [autoLoad, reload]);

  return { data, loading, error, reload, request, setData };
}
