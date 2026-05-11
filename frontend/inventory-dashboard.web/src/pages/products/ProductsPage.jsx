import { useState, useEffect } from "react";
import useCategories from "../../hooks/useCategories";
import useSuppliers from "../../hooks/useSuppliers";
import useProducts from "../../hooks/useProducts";
import PageTitle from "../../components/layout/PageTitle";
import ListToolbar from "../../components/layout/ListToolbar";
import ContentCard from "../../components/layout/ContentCard";
import PageLayout from "../../components/layout/PageLayout";
import DataTable from "../../components/table/DataTable";
import ProductCreatePage from "./ProductCreatePage";
import FormModal from "../../components/modals/FormModal";
import ProductDetailPage from "./ProductDetailPage";
import { formatCHF } from "../../utils/format";
import ConfirmDeleteModal from "../../components/modals/ConfirmDeleteModal";
export default function ProductsPage() {
  // State for selected filters
  const [categoryId, setCategoryId] = useState("");
  const [supplierId, setSupplierId] = useState("");
  //
  const [showModal, setShowModal] = useState(false);
  const [showDetailModal, setShowDetailModal] = useState(false);
  const [editId, setEditId] = useState(null);
  const [detailId, setDetailId] = useState(null);

  // Delete modal state
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [deleteId, setDeleteId] = useState(null);
  const [deleteName, setDeleteName] = useState("");
  const [deleting, setDeleting] = useState(false);

  const [page, setPage] = useState(1);
  const pageSize = 10;

  // Utilize the custom hooks to manage categories and suppliers
  const { categories, loadingCategories, loadCategories } = useCategories();
  const { suppliers, loadingSuppliers, loadSuppliers } = useSuppliers();

  const [search, setSearch] = useState("");

  // Utilize the custom hook to manage products
  const {
    products,
    loadingProducts,
    productsError,
    loadProducts,
    deleteProduct,
  } = useProducts();

  // State for filter visibility
  const [showFilter, setShowFilter] = useState(false);

  const handleDeleteProduct = (item) => {
    setDeleteId(item.productId);
    setDeleteName(item.productName); // kommt aus tableItems
    setShowDeleteModal(true);
  };

  // Fetch products from API (placeholder logic)
  useEffect(() => {
    // Initial load of products
    loadProducts({
      search: "",
      categoryId: "",
      supplierId: "",
      page: 1,
      pageSize: 10,
    });
    loadSuppliers({});
    loadCategories({});
  }, [loadProducts, loadSuppliers, loadCategories]);

  const tableItems = (products ?? []).map((p) => ({
    productId: p.productId,
    productName: p.productTitle,
    categoryName: p.categoryName,
    supplierName: p.supplierName,
    unitPrice: (
      <span className="d-block text-center">{formatCHF(p.price)}</span>
    ),
    unitsInStock: (
      <span className="d-block text-center">{p.quantityInStock ?? 0}</span>
    ),
    minimumStock: p.minimumStock ?? 0,
    stockStatus: getStockBadge(p.quantityInStock ?? 0, p.minimumStock ?? 0),
  }));

  return (
    <PageLayout>
      {/* Page header */}
      <PageTitle title="Products" />

      <FormModal
        show={showModal}
        onClose={() => {
          setShowModal(false);
          loadProducts({ search, categoryId, supplierId, page, pageSize });
        }}
        title={editId ? "Edit Product" : "New Product"}
      >
        <ProductCreatePage
          isModal
          productId={editId}
          onDone={() => {
            setShowModal(false);
            loadProducts({ search, categoryId, supplierId, page, pageSize });
          }}
        />
      </FormModal>
      <FormModal
        show={showDetailModal}
        onClose={() => setShowDetailModal(false)}
        title="Product Details"
      >
        <ProductDetailPage
          isModal
          productId={detailId}
          onDone={() => {
            setShowDetailModal(false);
          }}
        />
      </FormModal>

      <ConfirmDeleteModal
        show={showDeleteModal}
        title="Delete Product"
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
            await deleteProduct(deleteId);
            await loadProducts({
              search,
              categoryId,
              supplierId,
              page,
              pageSize,
            });
            setShowDeleteModal(false);
            setDeleteId(null);
            setDeleteName("");
          } finally {
            setDeleting(false);
          }
        }}
      />

      {/* Card for Products */}
      <ContentCard>
        <ListToolbar
          search={search}
          onSearchChange={setSearch}
          onSearchSubmit={() => {
            setPage(1);
            loadProducts({ search, categoryId, supplierId, page: 1, pageSize });
          }}
          Ad_ButtonLabel="Add Product"
          Ad_Action_Click={() => {
            setEditId(null);
            setShowModal(true);
          }}
        >
          {/* Filter button */}
          <div className="d-flex align-items-center gap-2">
            <button
              type="button"
              className="btn btn-outline-secondary btn-sm"
              onClick={() => setShowFilter((v) => !v)}
              disabled={loadingCategories || loadingSuppliers}
            >
              {showFilter ? "Close" : "Filter"}
            </button>
          </div>
          {showFilter && (
            <div className="row g-2 align-items-end mb-3 col-12">
              <div className="col-12 col-md-4">
                <label className="form-label small mb-1">Category</label>
                <select
                  className="form-select form-select-sm"
                  value={categoryId}
                  onChange={(e) => setCategoryId(e.target.value)}
                >
                  <option value="">All categories</option>
                  {(categories ?? []).map((c) => (
                    <option key={c.categoryId} value={c.categoryId}>
                      {c.name}
                    </option>
                  ))}
                </select>
              </div>

              <div className="col-12 col-md-4">
                <label className="form-label small mb-1">Supplier</label>
                <select
                  className="form-select form-select-sm"
                  value={supplierId}
                  onChange={(e) => setSupplierId(e.target.value)}
                >
                  <option value="">All suppliers</option>
                  {(suppliers ?? []).map((s) => (
                    <option key={s.supplierId} value={s.supplierId}>
                      {s.companyName}
                    </option>
                  ))}
                </select>
              </div>

              <div className="col-12 col-md-4 d-flex gap-2">
                <button
                  type="button"
                  className="btn btn-primary btn-sm"
                  onClick={() => {
                    setPage(1);
                    loadProducts({
                      search,
                      categoryId,
                      supplierId,
                      page: 1,
                      pageSize,
                    });
                  }}
                  disabled={loadingProducts}
                >
                  Apply
                </button>

                <button
                  type="button"
                  className="btn btn-outline-secondary btn-sm"
                  onClick={() => {
                    setCategoryId("");
                    setSupplierId("");
                    setSearch("");
                    // optional: auch search resetten
                    // setSearch("");
                    setPage(1);
                    loadProducts({
                      search: "",
                      categoryId: "",
                      supplierId: "",
                      page: 1,
                      pageSize,
                    });
                  }}
                  disabled={loadingProducts}
                >
                  Reset
                </button>
              </div>
            </div>
          )}
        </ListToolbar>

        {/* Table */}
        {loadingProducts && <p>Loading products...</p>}
        {productsError && <p className="text-danger">Error: {productsError}</p>}

        {!loadingProducts && !productsError && (
          <DataTable
            items={tableItems}
            rowKey="productId"
            labels={{
              productName: "Product Name",
              categoryName: "Category",
              supplierName: "Supplier",
              unitPrice: { text: "Price", className: "text-center" },
              unitsInStock: "Quantity",
              stockStatus: "Status",
            }}
            onEdit={(item) => {
              setEditId(item.productId);
              setShowModal(true);
            }}
            onDetail={(item) => {
              setDetailId(item.productId);
              setShowDetailModal(true);
            }}
            actions={{
              details: "/products/:id",
            }}
            onDelete={handleDeleteProduct}
            page={page}
            pageSize={pageSize}
            loading={loadingProducts}
            showPagination={true}
            onPageChange={(newPage) => {
              setPage(newPage);
              loadProducts({
                search,
                categoryId,
                supplierId,
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

function getStockBadge(quantity, minimumStock) {
  if (quantity === 0) {
    return <span className="badge bg-danger">No stock</span>;
  } else if (quantity < minimumStock) {
    return <span className="badge bg-warning text-dark">Low stock</span>;
  } else {
    return <span className="badge bg-success">In stock</span>;
  }
}
