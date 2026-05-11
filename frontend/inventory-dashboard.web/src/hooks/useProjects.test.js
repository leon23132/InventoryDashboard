import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act } from "@testing-library/react";
import useProjects from "./useProjects";
import useApiResource from "./useApiResource";

vi.mock("./useApiResource", () => ({
  default: vi.fn(),
}));

const mockReloadProjects = vi.fn();
const mockRequest = vi.fn();

const NotFoundError = { status: 404, message: "Not Found" };
const ApiError = new Error("API Error");
const ValidationError = { status: 400, message: "Validation Error" };
const NetworkError = new Error("Network Error");

describe("useProjects", () => {
  beforeEach(() => {
    vi.clearAllMocks();

    useApiResource.mockReturnValue({
      data: [
        {
          projectId: 1,
          projectName: "Website Relaunch",
          description: "Neues Firmenportal",
        },
      ],
      loading: false,
      error: null,
      reload: mockReloadProjects,
      request: mockRequest,
      setData: vi.fn(),
    });
  });

  describe("Get Project By ID", () => {
    it("Get Project By ID - Success", async () => {
      const mockProject = {
        projectId: 3,
        projectName: "Inventory Dashboard",
        description: "IPA Projekt",
      };

      mockRequest.mockResolvedValueOnce(mockProject);

      const { result } = renderHook(() => useProjects());

      let data;
      await act(async () => {
        data = await result.current.getProjectById(3);
      });

      expect(mockRequest).toHaveBeenCalledWith("/api/projects/3", {
        method: "GET",
      });
      expect(data).toEqual(mockProject);
    });

    it("Get Project By ID - Not Found", async () => {
      mockRequest.mockRejectedValueOnce(NotFoundError);

      const { result } = renderHook(() => useProjects());

      await expect(result.current.getProjectById(999)).rejects.toEqual(
        NotFoundError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/projects/999", {
        method: "GET",
      });
    });

    it("Get Project By ID - API Error", async () => {
      mockRequest.mockRejectedValueOnce(ApiError);

      const { result } = renderHook(() => useProjects());

      await expect(result.current.getProjectById(3)).rejects.toEqual(ApiError);

      expect(mockRequest).toHaveBeenCalledWith("/api/projects/3", {
        method: "GET",
      });
    });
  });

  describe("Load Projects with Filters", () => {
    it("Load Projects with Filters", async () => {
      const { result } = renderHook(() => useProjects());

      await act(async () => {
        await result.current.loadProjects({
          search: "Dashboard",
          page: 1,
          pageSize: 10,
        });
      });

      expect(mockReloadProjects).toHaveBeenCalledWith(
        "/api/projects?search=Dashboard&page=1&pageSize=10",
      );
    });

    it("Load Projects with Default Filters", async () => {
      const { result } = renderHook(() => useProjects());

      await act(async () => {
        await result.current.loadProjects();
      });

      expect(mockReloadProjects).toHaveBeenCalledWith(
        "/api/projects?page=1&pageSize=10",
      );
    });

    it("Load Projects - With Search Only", async () => {
      const { result } = renderHook(() => useProjects());

      await act(async () => {
        await result.current.loadProjects({ search: "Dashboard" });
      });

      expect(mockReloadProjects).toHaveBeenCalledWith(
        "/api/projects?search=Dashboard&page=1&pageSize=10",
      );
    });

    it("Load Projects - Trims Search", async () => {
      const { result } = renderHook(() => useProjects());

      await act(async () => {
        await result.current.loadProjects({ search: "  Dashboard  " });
      });

      expect(mockReloadProjects).toHaveBeenCalledWith(
        "/api/projects?search=Dashboard&page=1&pageSize=10",
      );
    });

    it("Load Projects - Invalid Page Uses Default", async () => {
      const { result } = renderHook(() => useProjects());

      await act(async () => {
        await result.current.loadProjects({ page: 0, pageSize: 10 });
      });

      expect(mockReloadProjects).toHaveBeenCalledWith(
        "/api/projects?page=1&pageSize=10",
      );
    });

    it("Load Projects - Invalid PageSize Uses Default", async () => {
      const { result } = renderHook(() => useProjects());

      await act(async () => {
        await result.current.loadProjects({ page: 1, pageSize: 0 });
      });

      expect(mockReloadProjects).toHaveBeenCalledWith(
        "/api/projects?page=1&pageSize=10",
      );
    });

    it("Load Projects - PageSize Above 100 Uses Max 100", async () => {
      const { result } = renderHook(() => useProjects());

      await act(async () => {
        await result.current.loadProjects({ page: 1, pageSize: 200 });
      });

      expect(mockReloadProjects).toHaveBeenCalledWith(
        "/api/projects?page=1&pageSize=100",
      );
    });

    it("Load Projects - Not Found", async () => {
      mockReloadProjects.mockRejectedValueOnce(NotFoundError);

      const { result } = renderHook(() => useProjects());

      await expect(result.current.loadProjects()).rejects.toEqual(
        NotFoundError,
      );

      expect(mockReloadProjects).toHaveBeenCalledWith(
        "/api/projects?page=1&pageSize=10",
      );
    });

    it("Load Projects - API Error", async () => {
      mockReloadProjects.mockRejectedValueOnce(ApiError);

      const { result } = renderHook(() => useProjects());

      await expect(result.current.loadProjects()).rejects.toEqual(ApiError);

      expect(mockReloadProjects).toHaveBeenCalledWith(
        "/api/projects?page=1&pageSize=10",
      );
    });
  });

  describe("Delete Project", () => {
    it("Delete Project - Success", async () => {
      mockRequest.mockResolvedValueOnce();
      mockReloadProjects.mockResolvedValueOnce();

      const { result } = renderHook(() => useProjects());

      let success;
      await act(async () => {
        success = await result.current.deleteProject(3);
      });

      expect(mockRequest).toHaveBeenCalledWith("/api/projects/3", {
        method: "DELETE",
      });
      expect(mockReloadProjects).toHaveBeenCalled();
      expect(success).toBe(true);
    });

    it("Delete Project - API Error", async () => {
      mockRequest.mockRejectedValueOnce(ApiError);

      const { result } = renderHook(() => useProjects());

      await expect(result.current.deleteProject(3)).rejects.toEqual(ApiError);

      expect(mockRequest).toHaveBeenCalledWith("/api/projects/3", {
        method: "DELETE",
      });
      expect(mockReloadProjects).not.toHaveBeenCalled();
    });

    it("Delete Project - Not Found", async () => {
      mockRequest.mockRejectedValueOnce(NotFoundError);

      const { result } = renderHook(() => useProjects());

      await expect(result.current.deleteProject(3)).rejects.toEqual(
        NotFoundError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/projects/3", {
        method: "DELETE",
      });
      expect(mockReloadProjects).not.toHaveBeenCalled();
    });
  });

  describe("Create Project", () => {
    it("Create Project - Success", async () => {
      const payload = {
        projectName: "Inventory Dashboard",
        description: "IPA Projekt",
      };

      const mockResponse = {
        projectId: 3,
        projectName: "Inventory Dashboard",
        description: "IPA Projekt",
      };

      mockRequest.mockResolvedValueOnce(mockResponse);

      const { result } = renderHook(() => useProjects());

      let data;
      await act(async () => {
        data = await result.current.createProject(payload);
      });

      expect(mockRequest).toHaveBeenCalledWith("/api/projects", {
        method: "POST",
        body: JSON.stringify(payload),
      });
      expect(data).toEqual(mockResponse);
    });

    it("Create Project - API Error", async () => {
      const payload = {
        projectName: "Inventory Dashboard",
        description: "IPA Projekt",
      };

      mockRequest.mockRejectedValueOnce(ApiError);

      const { result } = renderHook(() => useProjects());

      await expect(result.current.createProject(payload)).rejects.toEqual(
        ApiError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/projects", {
        method: "POST",
        body: JSON.stringify(payload),
      });
    });

    it("Create Project - Validation Error", async () => {
      const payload = {
        projectName: "",
        description: "IPA Projekt",
      };

      mockRequest.mockRejectedValueOnce(ValidationError);

      const { result } = renderHook(() => useProjects());

      await expect(result.current.createProject(payload)).rejects.toEqual(
        ValidationError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/projects", {
        method: "POST",
        body: JSON.stringify(payload),
      });
    });

    it("Create Project - Network Error", async () => {
      const payload = {
        projectName: "Inventory Dashboard",
        description: "IPA Projekt",
      };

      mockRequest.mockRejectedValueOnce(NetworkError);

      const { result } = renderHook(() => useProjects());

      await expect(result.current.createProject(payload)).rejects.toEqual(
        NetworkError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/projects", {
        method: "POST",
        body: JSON.stringify(payload),
      });
    });
  });

  describe("Update Project", () => {
    it("Update Project - Success", async () => {
      const payload = {
        projectName: "Updated Inventory Dashboard",
        description: "Updated IPA Projekt",
      };

      const updatedProject = {
        projectId: 3,
        projectName: "Updated Inventory Dashboard",
        description: "Updated IPA Projekt",
      };

      mockRequest.mockResolvedValueOnce(updatedProject);

      const { result } = renderHook(() => useProjects());

      let data;
      await act(async () => {
        data = await result.current.updateProject(3, payload);
      });

      expect(mockRequest).toHaveBeenCalledWith("/api/projects/3", {
        method: "PUT",
        body: JSON.stringify(payload),
      });
      expect(data).toEqual(updatedProject);
    });

    it("Update Project - Not Found", async () => {
      const payload = {
        projectName: "Updated Inventory Dashboard",
        description: "Updated IPA Projekt",
      };

      mockRequest.mockRejectedValueOnce(NotFoundError);

      const { result } = renderHook(() => useProjects());

      await expect(result.current.updateProject(999, payload)).rejects.toEqual(
        NotFoundError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/projects/999", {
        method: "PUT",
        body: JSON.stringify(payload),
      });
    });

    it("Update Project - Validation Error", async () => {
      const payload = {
        projectName: "",
        description: "Updated IPA Projekt",
      };

      mockRequest.mockRejectedValueOnce(ValidationError);

      const { result } = renderHook(() => useProjects());

      await expect(result.current.updateProject(3, payload)).rejects.toEqual(
        ValidationError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/projects/3", {
        method: "PUT",
        body: JSON.stringify(payload),
      });
    });

    it("Update Project - API Error", async () => {
      const payload = {
        projectName: "Updated Inventory Dashboard",
        description: "Updated IPA Projekt",
      };

      mockRequest.mockRejectedValueOnce(ApiError);

      const { result } = renderHook(() => useProjects());

      await expect(result.current.updateProject(3, payload)).rejects.toEqual(
        ApiError,
      );

      expect(mockRequest).toHaveBeenCalledWith("/api/projects/3", {
        method: "PUT",
        body: JSON.stringify(payload),
      });
    });
  });
});
