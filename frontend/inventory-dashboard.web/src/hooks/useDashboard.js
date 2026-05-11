import useApiResource from "./useApiResource";
import { useCallback } from "react";

export default function useDashboard() {
  const {
    data: overview,
    loading: loadingOverview,
    error: overviewError,
    reload: reloadOverview,
    request,
    setData: setOverview,
  } = useApiResource("/api/Dashboard/overview");

  // Function to load the dashboard overview data
  const loadOverview = useCallback(() => {
    return reloadOverview("/api/Dashboard/overview");
  }, [reloadOverview]);

  // Function to fetch the dashboard overview data without automatically updating state
  const getOverview = useCallback(async () => {
    const data = await request("/api/Dashboard/overview", {
      method: "GET",
      updateState: true,
    });
    return data;
  }, [request]);

  return {
    overview,
    loadingOverview,
    overviewError,
    reloadOverview,
    loadOverview,
    getOverview,
    setOverview,
  };
}
