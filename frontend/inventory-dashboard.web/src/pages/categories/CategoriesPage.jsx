import { useEffect, useState } from "react";
import useCategories from "../../hooks/useCategories";
import PageTitle from "../../components/layout/PageTitle";
import ListToolbar from "../../components/layout/ListToolbar";
import PageLayout from "../../components/layout/PageLayout";
import ContentCard from "../../components/layout/ContentCard";
import DataTable from "../../components/table/DataTable";
import FormModal from "../../components/modals/FormModal";
import CategoryCreatePage from "./CategoryCreatePage";
import CategoryDetailPage from "./CategoryDetailPage";
import ConfirmDeleteModal from "../../components/modals/ConfirmDeleteModal";

export default function CategoriesPage() {
  const [search, setSearch] = useState("");

  const [showModal, setShowModal] = useState(false);
  const [showDetailModal, setShowDetailModal] = useState(false);
  const [editId, setEditId] = useState(null);
  const [detailId, setDetailId] = useState(null);

  // Delete modal state
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [deleteId, setDeleteId] = useState(null);
  const [deleteName, setDeleteName] = useState("");
  const [deleting, setDeleting] = useState(false);

  const {
    categories,
    loadingCategories,
    categoriesError,
    loadCategories,
    deleteCategory,
  } = useCategories();
  // State for filter visibility

  const [page, setPage] = useState(1);
  const pageSize = 3;

  const applyFilters = () => {
    setPage(1); // Reset to first page when applying new filters
    loadCategories({ search, page: 1, pageSize });
  };

  useEffect(() => {
    // Initial load of categories
    loadCategories({ search: "", page: 1, pageSize });
  }, [loadCategories]);

  const handleDeleteCategory = (item) => {
    setDeleteId(item.categoryId);
    setDeleteName(item.categoryName);
    setShowDeleteModal(true);
  };

  // Prepare table items
  const tableItems = (categories ?? []).map((c) => ({
    categoryId: c.categoryId,
    categoryName: c.name,
    description: c.description,
  }));

  return (
    <PageLayout>
      {/* Page header */}
      <PageTitle title="Categories" />
      <FormModal
        show={showModal}
        onClose={() => {
          setShowModal(false);
          loadCategories({ search, page, pageSize });
        }}
        title={editId ? "Edit Category" : "Add Category"}
      >
        <CategoryCreatePage
          isModal={true}
          categoryId={editId}
          onDone={() => {
            setShowModal(false);
            loadCategories({ search, page, pageSize });
          }}
        />
      </FormModal>
      <FormModal
        show={showDetailModal}
        onClose={() => setShowDetailModal(false)}
        title="Category Details"
      >
        <CategoryDetailPage
          isModal={true}
          categoryId={detailId}
          onDone={() => setShowDetailModal(false)}
        />
      </FormModal>

      <ConfirmDeleteModal
        show={showDeleteModal}
        title="Delete Category"
        message={`Do you really want to delete "${deleteName}"? This action cannot be undone.`}
        confirmText="Delete"
        onCancel={() => {
          if (deleting) return;
          setShowDeleteModal(false);
          setDeleteId(null);
          setDeleteName("");
        }}
        loading={deleting}
        onConfirm={async () => {
          if (!deleteId) return;

          try {
            setDeleting(true);
            await deleteCategory(deleteId);
            await loadCategories({ search, page, pageSize });
            setShowDeleteModal(false);
            setDeleteId(null);
            setDeleteName("");
          } finally {
            setDeleting(false);
          }
        }}
      />

      {/*Card*/}
      <ContentCard>
        <ListToolbar
          search={search}
          onSearchChange={setSearch}
          onSearchSubmit={applyFilters}
          placeholder="Search categories..."
          count={(categories ?? []).length}
          Ad_ButtonLabel="Add Category"
          Ad_Action_Click={() => {
            setEditId(null);
            setShowModal(true);
          }}
        ></ListToolbar>

        {/* Suppliers Table */}
        {loadingCategories && <p>Loading categories...</p>}
        {categoriesError && (
          <p className="text-danger">Error:{categoriesError}</p>
        )}
        {/* If no errors and not loading, show categories */}
        {!loadingCategories && !categoriesError && (
          <DataTable
            items={tableItems}
            rowKey="categoryId"
            labels={{
              categoryName: "Category Name",
            }}
            onEdit={(item) => {
              setEditId(item.categoryId);
              setShowModal(true);
            }}
            onDetail={(item) => {
              setDetailId(item.categoryId);
              setShowDetailModal(true);
            }}
            actions={{
              edit: "/categories/:id/edit",
              details: "/categories/:id",
            }}
            onDelete={handleDeleteCategory}
            page={page}
            pageSize={pageSize}
            loading={loadingCategories}
            showPagination={true}
            onPageChange={(newPage) => {
              setPage(newPage);
              loadCategories({
                search,
                page: newPage,
                pageSize,
              });
            }}
          />
        )}
      </ContentCard>
    </PageLayout>
  );
}
