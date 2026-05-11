import { useEffect, useState } from "react";
import useSuppliers from "../../hooks/useSuppliers";
import PageTitle from "../../components/layout/PageTitle";
import ListToolbar from "../../components/layout/ListToolbar";
import PageLayout from "../../components/layout/PageLayout";
import ContentCard from "../../components/layout/ContentCard";
import DataTable from "../../components/table/DataTable";
import FormModal from "../../components/modals/FormModal";
import SupplierCreatePage from "./SupplierCreatePage";
import SupplierDetailPage from "./SupplierDetailPage";
import ConfirmDeleteModal from "../../components/modals/ConfirmDeleteModal";

export default function SuppliersPage() {
  const [search, setSearch] = useState("");
  const [contactPerson, setContactPerson] = useState("");
  const [city, setCity] = useState("");

  // Delete modal state
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [deleteId, setDeleteId] = useState(null);
  const [deleteName, setDeleteName] = useState("");
  const [deleting, setDeleting] = useState(false);
  const [deleteError, setDeleteError] = useState("");

  const [page, setPage] = useState(1);
  const pageSize = 10;

  const {
    suppliers,
    loadingSuppliers,
    suppliersError,
    loadSuppliers,
    deleteSupplier,
  } = useSuppliers();
  // State for filter visibility
  const [showFilter, setShowFilter] = useState(false);

  // Unique list of contact persons from suppliers
  const contactPersons = Array.from(
    new Set((suppliers ?? []).map((s) => s.contactPerson).filter(Boolean)),
  ).sort();

  // Unique list of cities from suppliers' billing addresses
  const cities = Array.from(
    new Set(
      (suppliers ?? []).map((s) => s.billingAddress?.city).filter(Boolean),
    ),
  ).sort();

  const applyFilters = () => {
    setPage(1);
    loadSuppliers({ search, contactPerson, city, page: 1, pageSize });
  };
  const handleDeleteSupplier = (item) => {
    setDeleteId(item.supplierId);
    setDeleteName(item.companyName);
    setDeleteError("");
    setShowDeleteModal(true);
  };

  useEffect(() => {
    // Initial load of suppliers
    loadSuppliers({
      search: "",
      contactPerson: "",
      city: "",
      page: 1,
      pageSize: 10,
    });
  }, [loadSuppliers]);

  const [showModal, setShowModal] = useState(false);
  const [showDetailModal, setShowDetailModal] = useState(false);
  const [editId, setEditId] = useState(null);
  const [detailId, setDetailId] = useState(null);

  const tableItems = (suppliers ?? []).map((s) => ({
    supplierId: s.supplierId,
    companyName: s.companyName,
    contactPerson: s.contactPerson,
    city: s.billingAddress?.city || "",
    phoneNumber: s.phoneNumber,
    email: s.email,
    streetAddress: s.billingAddress?.streetAddress ?? "",
    website: s.website,
  }));

  return (
    <PageLayout>
      {/* Page header */}
      <PageTitle title="Suppliers" />

      <FormModal
        show={showModal}
        onClose={() => {
          setShowModal(false);
          loadSuppliers({ search, contactPerson, city, page, pageSize });
        }}
        title={editId ? "Edit Supplier" : "New Supplier"}
      >
        <SupplierCreatePage
          isModal
          supplierId={editId}
          onDone={() => {
            setShowModal(false);
            loadSuppliers({ search, contactPerson, city, page, pageSize });
          }}
        />
      </FormModal>
      <FormModal
        show={showDetailModal}
        onClose={() => setShowDetailModal(false)}
        title="Supplier Details"
      >
        <SupplierDetailPage
          supplierId={detailId}
          isModal
          onDone={() => {
            setShowDetailModal(false);
            loadSuppliers({ search, contactPerson, city, page, pageSize });
          }}
        />
      </FormModal>

      <ConfirmDeleteModal
        show={showDeleteModal}
        title="Delete Supplier"
        message={`Do you really want to delete "${deleteName}"? This action cannot be undone.`}
        confirmText="Delete"
        error={deleteError}
        onCancel={() => {
          if (deleting) return;
          setShowDeleteModal(false);
          setDeleteId(null);
          setDeleteName("");
          setDeleteError("");
        }}
        loading={deleting}
        onConfirm={async () => {
          if (!deleteId) return;
          try {
            setDeleting(true);
            await deleteSupplier(deleteId);
            await loadSuppliers({
              search,
              contactPerson,
              city,
              page,
              pageSize,
            });
            setShowDeleteModal(false);
            setDeleteId(null);
            setDeleteName("");
            setDeleteError("");
          } catch (error) {
            setDeleteError(
              error.message || "An error occurred while deleting the supplier.",
            );
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
          Ad_ButtonLabel="Add Supplier"
          Ad_Action_Click={() => {
            setEditId(null);
            setShowModal(true);
          }}
        >
          {/* Filter Button */}
          <div className="d-flex align-items gap-2">
            <button
              type="button"
              className="btn btn-outline-secondary btn-sm"
              onClick={() => setShowFilter(!showFilter)}
            >
              {showFilter ? "Close" : "Filter"}
            </button>
          </div>

          {/* Filter Section */}
          {showFilter && (
            <div className="row g-2 align-items-end mb-3 col-12">
              {/* Contact Person Filter */}
              <div className="col-12 col-md-4">
                <label className="form-label small mb-1">Contact Person</label>
                <select
                  className="form-select form-select-sm"
                  value={contactPerson}
                  onChange={(e) => setContactPerson(e.target.value)}
                >
                  <option value="">All Contact Persons</option>
                  {(contactPersons ?? []).map((cp) => (
                    <option key={cp} value={cp}>
                      {cp}
                    </option>
                  ))}
                </select>
              </div>
              {/* City Filter */}
              <div className="col-12 col-md-4">
                <label className="form-label small mb-1">City</label>
                <select
                  className="form-select form-select-sm"
                  value={city}
                  onChange={(e) => setCity(e.target.value)}
                >
                  <option value="">All Cities</option>
                  {(cities ?? []).map((city) => (
                    <option key={city} value={city}>
                      {city}
                    </option>
                  ))}
                </select>
              </div>
              <div className="col-12 col-md-4 gap-2 d-flex">
                <button
                  type="button"
                  className="btn btn-primary btn-sm"
                  onClick={() =>
                    loadSuppliers({
                      search,
                      contactPerson,
                      city,
                      page,
                      pageSize,
                    })
                  }
                  disabled={loadingSuppliers}
                >
                  Apply
                </button>
                <button
                  type="button"
                  className="btn btn-secondary btn-sm"
                  onClick={() => {
                    setSearch("");
                    setContactPerson("");
                    setCity("");
                    loadSuppliers({
                      search: "",
                      contactPerson: "",
                      city: "",
                      page: 1,
                      pageSize,
                    });
                  }}
                >
                  Clear
                </button>
              </div>
            </div>
          )}
        </ListToolbar>

        {/*  */}

        {/* Suppliers Table */}
        {loadingSuppliers && <p>Loading suppliers...</p>}
        {suppliersError && (
          <p className="text-danger">Error:{suppliersError}</p>
        )}
        {/* If no errors and not loading, show suppliers */}
        {!loadingSuppliers && !suppliersError && (
          <DataTable
            items={tableItems}
            rowKey="supplierId"
            labels={{
              companyName: "Company Name",
              contactPerson: "Contact",
              city: "City",
              phoneNumber: "Phone",
              email: "Email",
              streetAddress: "Address",
              website: "Website",
            }}
            onEdit={(item) => {
              setEditId(item.supplierId);
              setShowModal(true);
            }}
            onDetail={(item) => {
              setDetailId(item.supplierId);
              setShowDetailModal(true);
            }}
            actions={{
              edit: "/suppliers/:id/edit",
              details: "/suppliers/:id",
            }}
            onDelete={handleDeleteSupplier}
            page={page}
            pageSize={pageSize}
            loading={loadingSuppliers}
            showPagination={true}
            onPageChange={(newPage) => {
              setPage(newPage);
              loadSuppliers({
                search,
                contactPerson,
                city,
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
