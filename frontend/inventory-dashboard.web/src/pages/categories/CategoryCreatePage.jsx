import { useParams, useNavigate } from "react-router-dom";
import useCategories from "../../hooks/useCategories";
import { useEffect, useState } from "react";
import PageTitle from "../../components/layout/PageTitle";
import PageLayout from "../../components/layout/PageLayout";
import ContentCard from "../../components/layout/ContentCard";
import { validate } from "../../utils/validator";
export default function CategoryCreatePage({
  isModal = false,
  categoryId = null,
  onDone,
}) {

  const params = useParams();
  const navigate = useNavigate();

  const effectiveId = categoryId ?? params.Id;
  const isEditMode = Boolean(effectiveId);

  // State for form data and loading status
  const [categoryLoading, setCategoryLoading] = useState(false);
  const [categoryError, setCategoryError] = useState(null);

  const [categoryFormData, setCategoryFormData] = useState({
    name: "",
  });
  // Custom hook to manage categories
  const { createCategory, updateCategory, getCategoryById } = useCategories();

  const categorySchema = {
    name: ["required"],
  };

  const emptyErrors = {
    name: "",
  };
  // State for form errors
  const [formErrors, setFormErrors] = useState(emptyErrors);

  const handleSave = async (e) => {
    e.preventDefault();
    try {
      setCategoryLoading(true);
      setCategoryError(null);
      // Validate all fields before submission
      const { errors, hasErrors } = validate(categoryFormData, categorySchema);
      setFormErrors({ ...emptyErrors, ...errors });
      if (hasErrors) return;
      // Prepare payload for API
      const payload = {
        name: categoryFormData.name,
      };

      // Call appropriate API based on mode
      if (isEditMode) {
        await updateCategory(effectiveId, payload);
      } else {
        await createCategory(payload);
      }
      if (isModal && onDone) onDone();
      navigate("/categories");
    } catch (error) {
      // Handle any errors during save
      setCategoryError(error.message ?? "Save failed");
    } finally {
      setCategoryLoading(false);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;

    setCategoryFormData((prev) => {
      const next = { ...prev, [name]: value };

      // Validate only this field
      const { errors } = validate(next, { [name]: categorySchema[name] });

      setFormErrors((prevErrors) => {
        const msg = errors[name];

        if (!msg) {
          const { [name]: _, ...rest } = prevErrors;
          return rest;
        }

        return { ...prevErrors, [name]: msg };
      });

      return next;
    });
  };

  useEffect(() => {
    if (!isEditMode) return;
    setCategoryLoading(true);
    getCategoryById(effectiveId)
      .then((data) => {
        setCategoryFormData({
          name: data.name,
        });
      })
      .catch((error) => setCategoryError(error.message ?? "Load failed"))
      .finally(() => setCategoryLoading(false));
  }, [effectiveId, isEditMode, getCategoryById]);

  return (
    <PageLayout>
      {/* Page header */}
      <PageTitle title={isEditMode ? "Edit Category" : "Create Category"} />

      {/* Form for creating or editing a category */}
      {categoryError && (
        <div className="alert alert-danger">{categoryError}</div>
      )}
      {categoryLoading && <div>Loading category data...</div>}

      <ContentCard>
        <form className="row g-3" onSubmit={handleSave}>
          {/* Category Name */}
          <div className="col-md-6">
            <label className="form-label">Category Name</label>
            {/* Error message for name field */}
            {formErrors.name && (
              <div className="text-danger mb-1">{formErrors.name}</div>
            )}
            <input
              type="text"
              name="name"
              className={`form-control ${formErrors.name ? "is-invalid" : ""}`}
              value={categoryFormData.name}
              onChange={handleChange}
            />
          </div>
          {/* Form actions */}
          <div className="col-12 d-flex gap-2 mt-2">
            <button
              type="submit"
              className="btn btn-primary"
              disabled={categoryLoading}
            >
              Save
            </button>

            <button
              type="button"
              className="btn btn-outline-secondary"
              onClick={() => {
                if (isModal && onDone) {
                  onDone();
                } else {
                  navigate("/categories");
                }
              }}
            >
              Cancel
            </button>
          </div>
        </form>
      </ContentCard>
    </PageLayout>
  );
}
