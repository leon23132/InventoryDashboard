import PageTitle from "../../components/layout/PageTitle";
import PageLayout from "../../components/layout/PageLayout";
import ListToolbar from "../../components/layout/ListToolbar";
import ContentCard from "../../components/layout/ContentCard";

import { formatCHF } from "../../utils/format";
import { useState, useEffect } from "react";
import useProjects from "../../hooks/useProjects";
import DataTable from "../../components/table/DataTable";
import FormModal from "../../components/modals/FormModal";
import ProjectCreatePage from "./ProjectCreatePage";
import ProjectDetailPage from "./ProjectDetailPage";
import ConfirmDeleteModal from "../../components/modals/ConfirmDeleteModal";
export default function ProjectsPage() {
  // State for search term
  const [search, setSearch] = useState("");

  const {
    projects,
    loadingProjects,
    projectsError,
    loadProjects,
    deleteProject,
  } = useProjects();

  // Create/Edit modal
  const [showModal, setShowModal] = useState(false);
  const [editId, setEditId] = useState(null);

  // Detail modal
  const [showDetailModal, setShowDetailModal] = useState(false);
  const [detailId, setDetailId] = useState(null);

  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [deleteId, setDeleteId] = useState(null);
  const [deleteName, setDeleteName] = useState("");
  const [deleting, setDeleting] = useState(false);

  const [page, setPage] = useState(1);
  const pageSize = 5;

  useEffect(() => {
    loadProjects({ search: "", page: 1, pageSize });
  }, [loadProjects]);

  const handleDeleteProject = (project) => {
    setDeleteId(project.projectId);
    setDeleteName(project.projectName);
    setShowDeleteModal(true);
  };

  return (
    <PageLayout>
      {/* Page header */}
      <PageTitle title="Projects" />

      {/* Modals */}
      <FormModal
        show={showModal}
        onClose={() => {
          setShowModal(false);
          loadProjects({ search, page, pageSize });
        }}
        title={editId ? "Edit Project" : "New Project"}
      >
        <ProjectCreatePage
          isModal
          projectId={editId}
          onDone={() => {
            setShowModal(false);
            loadProjects({ search, page, pageSize });
          }}
        />
      </FormModal>

      <FormModal
        show={showDetailModal}
        onClose={() => setShowDetailModal(false)}
        title="Project Details"
      >
        <ProjectDetailPage
          projectId={detailId}
          isModal
          onDone={() => {
            setShowDetailModal(false);
            loadProjects({ search, page, pageSize });
          }}
        />
      </FormModal>

      <ConfirmDeleteModal
        show={showDeleteModal}
        title="Delete Project"
        message={`Do you really want to delete "${deleteName}"? This action cannot be undone.`}
        confirmText="Delete"
        onCancel={() => {
          setShowDeleteModal(false);
          setDeleteId(null);
          setDeleteName("");
        }}
        loading={deleting}
        onConfirm={async () => {
          try {
            setDeleting(true);
            await deleteProject(deleteId);
            await loadProjects({ search, page, pageSize });
            setShowDeleteModal(false);
            setDeleteId(null);
            setDeleteName("");
          } finally {
            setDeleting(false);
          }
        }}
      />

      <div className="d-flex flex-column gap-3">
        <ContentCard>
          <ListToolbar
            placeholder="Search projects..."
            Ad_ButtonLabel="Add Project"
            onSearchChange={setSearch}
            onSearchSubmit={() => {
              setPage(1);
              loadProjects({ search, page: 1, pageSize });
            }}
            Ad_Action_Click={() => {
              setEditId(null);
              setShowModal(true);
            }}
          ></ListToolbar>
        </ContentCard>
        {projectsError && (
          <div className="alert alert-danger mb-0">Error: {projectsError}</div>
        )}
        {!loadingProjects && projects && projects.length === 0 && (
          <div className="alert alert-info mb-0">No projects found.</div>
        )}
        {projects.map((project) => (
          <ContentCard
            disableHeader={false}
            cardHeader={
              <div className="d-flex justify-content-between align-items-start">
                <div>
                  <h2 className="h5 mb-1">{project.projectName}</h2>
                  <div className="text-muted small">{project.description}</div>
                </div>
                <div className="d-flex gap-2">
                  <button
                    className="btn btn-primary btn-sm d-flex align-items-center gap-1 shadow-sm"
                    onClick={() => {
                      setEditId(project.projectId);
                      setShowModal(true);
                    }}
                  >
                    Edit
                  </button>
                  <button
                    className="btn btn-outline-primary btn-sm d-flex align-items-center gap-1 shadow-sm"
                    onClick={() => {
                      setDetailId(project.projectId);
                      setShowDetailModal(true);
                    }}
                  >
                    Details
                  </button>
                  <button
                    className="btn btn-danger btn-sm d-flex align-items-center gap-1 shadow-sm"
                    onClick={() => handleDeleteProject(project)}
                  >
                    Delete
                  </button>
                </div>
              </div>
            }
          >
            {project.products.length === 0 && (
              <div className="alert alert-info mb-0">
                No products in this project.
              </div>
            )}
            {project.products.length === 0 ? (
              <div className="alert alert-info mb-0">
                No products in this project.
              </div>
            ) : (
              <div className="table-responsive">
                <table className="table table-sm align-middle mb-0 table-spaced">
                  <thead className="table-light">
                    <tr>
                      <th className="text-center">Product Name</th>
                      <th className="text-center">Quantity</th>
                      <th className="text-end">Price Total</th>
                      <th className="text-end">Price Per Unit</th>
                    </tr>
                  </thead>

                  <tbody>
                    {project.products.map((product) => (
                      <tr key={product.productId}>
                        <td className="text-center">{product.productTitle}</td>
                        <td className="text-center">{product.quantity}</td>
                        <td className="text-end">
                          {formatCHF(product.totalPrice)}
                        </td>
                        <td className="text-end">
                          {formatCHF(product.unitPrice)}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </ContentCard>
        ))}
      </div>
      <div className="d-flex justify-content-between align-items-center mt-3">
        <button
          type="button"
          className="btn btn-outline-secondary btn-sm"
          disabled={page === 1 || loadingProjects}
          onClick={() => {
            const newPage = page - 1;
            setPage(newPage);
            loadProjects({ search, page: newPage, pageSize });
          }}
        >
          Previous
        </button>

        <span className="small text-muted">Page {page}</span>

        <button
          type="button"
          className="btn btn-outline-secondary btn-sm"
          disabled={loadingProjects || (projects?.length ?? 0) < pageSize}
          onClick={() => {
            const newPage = page + 1;
            setPage(newPage);
            loadProjects({ search, page: newPage, pageSize });
          }}
        >
          Next
        </button>
      </div>
    </PageLayout>
  );
}
