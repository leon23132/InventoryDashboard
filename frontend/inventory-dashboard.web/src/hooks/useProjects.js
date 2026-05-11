import React from "react";
import useApiResource from "./useApiResource";
import { useCallback } from "react";

function buildProjectsURL(search, page, pageSize) {
  // Construct query parameters based on provided filters
  const params = new URLSearchParams();

  // Append query parameters if they are provided
  if (search?.trim()) params.set("search", search.trim());

  params.set("page", page > 0 ? page : 1); // Default to page 1 if invalid
  params.set("pageSize", pageSize > 0 ? Math.min(pageSize, 100) : 10); // Default to 10, max 100
  // Construct the final URL with query string
  const qs = params.toString();
  return `/api/projects${qs ? `?${qs}` : ""}`;
}

export default function useProjects() {
  const {
    data: projects,
    loading: loadingProjects,
    error: projectsError,
    reload: reloadProjects,
    request,
  } = useApiResource("/api/projects", { autoLoad: false });

  // Function to load projects with optional filters
  const loadProjects = useCallback(
    ({ search, page = 1, pageSize = 10 } = {}) => {
      // Build the URL with the provided filters
      const path = buildProjectsURL(search, page, pageSize);
      // Promise to reload projects from the constructed URL
      return reloadProjects(path);
    },
    [reloadProjects],
  );

  // Function to delete a project
  const deleteProject = useCallback(
    async (projectId) => {
      // Send DELETE request to the API
      await request(`/api/projects/${projectId}`, { method: "DELETE" });
      // Refresh the projects list after deletion
      reloadProjects();
      return true;
    },
    [request, reloadProjects],
  );

  // Function to get project by ID
  const getProjectById = useCallback(
    async (projectId) => {
      // Find and return the project with the matching ID
      const data = await request(`/api/projects/${projectId}`, {
        method: "GET",
      });
      return data;
    },
    [request],
  );

  // Function to create a new project
  const createProject = useCallback(
    async (payload) => {
      return await request(`/api/projects`, {
        method: "POST",
        body: JSON.stringify(payload),
      });
    },
    [request],
  );
  // Function to update an existing project
  const updateProject = useCallback(
    async (projectId, payload) => {
      return await request(`/api/projects/${projectId}`, {
        method: "PUT",
        body: JSON.stringify(payload),
      });
    },
    [request],
  );

  return {
    projects: projects,
    loadingProjects: loadingProjects,
    projectsError: projectsError,
    loadProjects: loadProjects,
    createProject: createProject,
    deleteProject: deleteProject,
    getProjectById: getProjectById,
    updateProject: updateProject,
  };
}
