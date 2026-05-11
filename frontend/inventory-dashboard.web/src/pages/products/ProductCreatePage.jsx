import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { API_BASE_URL } from "../../config/config";
import useProducts from "../../hooks/useProducts";
import useCategories from "../../hooks/useCategories";
import useSuppliers from "../../hooks/useSuppliers";
import PageTitle from "../../components/layout/PageTitle";
import { validate } from "../../utils/validator";

export default function ProductCreatePage({
  isModal = false,
  productId = null,
  onDone,
}) {
  // State to handle loading and error states
  const [productLoading, setProductLoading] = useState(false);
  const [productError, setProductError] = useState(null);

  // Get supplier ID from URL params to determine if it's edit mode
  const params = useParams();
  const navigate = useNavigate();

  const effectiveId = productId ?? params.Id;
  const isEditMode = Boolean(effectiveId);

  const productSchema = {
    productTitle: [
      ["required", "Product name is required"],
      [
        "maxLength",
        {
          length: 100,
          message: "Product name must be at most 100 characters long",
        },
      ],
    ],
    price: [
      ["required", "Price is required"],
      ["number", "Price must be a valid number"],
      ["greaterThan", { min: 0, message: "Price must be greater than 0" }],
    ],
    categoryId: [["required", "Category is required"]],
    supplierId: [["required", "Supplier is required"]],
    quantityInStock: [
      ["required", "Quantity is required"],
      ["integer", "Quantity must be a whole number"],
      ["minNumber", { min: 0, message: "Quantity cannot be negative" }],
    ],
    minimumStock: [
      ["required", "Minimum stock is required"],
      ["integer", "Minimum stock must be a whole number"],
      ["minNumber", { min: 0, message: "Minimum stock cannot be negative" }],
    ],
    location: [
      ["required", "Location is required"],
      [
        "minLength",
        { length: 2, message: "Location must be at least 2 characters long" },
      ],
      [
        "maxLength",
        {
          length: 100,
          message: "Location must be at most 100 characters long",
        },
      ],
    ],
  };

  const {
    getProductById,
    request: productRequest,
    createProduct,
    updateProduct,
  } = useProducts();
  const { categories, loadingCategories, categoriesError, loadCategories } =
    useCategories();
  const { suppliers, loadingSuppliers, suppliersError, loadSuppliers } =
    useSuppliers();

  // Form state
  const [productForm, setProductForm] = useState({
    productId: "",
    productTitle: "",
    productDescription: "",
    categoryId: "",
    supplierId: "",
    price: "",
    quantityInStock: "",
    minimumStock: "",
    location: "",
  });

  // Form validation errors
  const [formErrors, setFormErrors] = useState({});

  useEffect(() => {
    // Load categories and suppliers for the dropdowns
    loadCategories();
    loadSuppliers();
  }, [loadCategories, loadSuppliers]);

  useEffect(() => {
    // If in edit mode, fetch the existing product data
    if (!isEditMode) return;

    getProductById(effectiveId).then((data) => {
      setProductForm({
        productId: data.productId,
        productTitle: data.productTitle,
        productDescription: data.productDescription,
        categoryId: String(data.categoryId),
        supplierId: String(data.supplierId),
        price: String(data.price),
        quantityInStock: String(data.quantityInStock),
        minimumStock: String(data.minimumStock),
        location: data.location?.trim() || "",
      });
    });
  }, [effectiveId, isEditMode, getProductById, loadCategories, loadSuppliers]);

  const handleSave = async (e) => {
    e.preventDefault();
    try {
      // Indicate loading state
      setProductLoading(true);
      setProductError(null);

      // Validate all fields before submission
      const { errors, hasErrors } = validate(productForm, productSchema);
      setFormErrors(errors);
      if (hasErrors) return;

      // Prepare payload for API
      const payload = {
        productTitle: productForm.productTitle,
        productDescription: productForm.productDescription,
        categoryId: Number(productForm.categoryId),
        supplierId: Number(productForm.supplierId),
        price: parseFloat(productForm.price),
        quantityInStock: parseInt(productForm.quantityInStock, 10),
        minimumStock: parseInt(productForm.minimumStock, 10),
        location: productForm.location?.trim() || null,
      };

      // Determine API endpoint and method based on mode

      if (isEditMode) {
        await updateProduct(effectiveId, payload);
      } else {
        await createProduct(payload);
      }

      if (isModal) {
        onDone?.();
      } else {
        navigate("/products");
      }
    } catch (error) {
      // Handle errors
      setProductError(error.message);
    } finally {
      // Reset loading state
      setProductLoading(false);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;

    setProductForm((prev) => {
      const next = { ...prev, [name]: value };

      const { errors } = validate(next, { [name]: productSchema[name] });

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

  return (
    <div className="container-fluid py-3">
      <PageTitle title={isEditMode ? "Edit Product" : "Create Product"} />

      {/** Error and loading messages */}
      {productError && <div className="alert alert-danger">{productError}</div>}
      {productLoading && <div>Loading product data...</div>}

      {/* Card */}
      <div className="card shadow-sm">
        <div className="card-body">
          <form className="row g-3" onSubmit={handleSave}>
            {/* Product name */}
            <div className="col-md-6">
              <label className="form-label">Product name</label>
              {/*Error message for price field */}
              {formErrors.productTitle && (
                <div className="text-danger mb-1">
                  {formErrors.productTitle}
                </div>
              )}
              <input
                name="productTitle"
                type="text"
                className={`form-control ${formErrors.productTitle ? "is-invalid" : ""}`}
                value={productForm.productTitle}
                onChange={handleChange}
              />
            </div>

            {/* Price */}
            <div className="col-md-3">
              <label className="form-label">Price</label>
              {/*Error message for price field */}
              {formErrors.price && (
                <div className="text-danger mb-1">{formErrors.price}</div>
              )}
              <input
                name="price"
                type="number"
                className={`form-control ${formErrors.price ? "is-invalid" : ""}`}
                value={productForm.price}
                onChange={handleChange}
              />
            </div>

            {/* Quantity */}
            <div className="col-md-3">
              <label className="form-label">Quantity</label>
              {/*Error message for quantity field */}
              {formErrors.quantityInStock && (
                <div className="text-danger mb-1">
                  {formErrors.quantityInStock}
                </div>
              )}
              <input
                type="number"
                name="quantityInStock"
                className={`form-control ${formErrors.quantityInStock ? "is-invalid" : ""}`}
                value={productForm.quantityInStock}
                onChange={handleChange}
              />
            </div>

            {/* Minimum Stock */}
            <div className="col-md-3">
              <label className="form-label">Minimum Stock</label>
              {/*Error message for minimum stock field */}
              {formErrors.minimumStock && (
                <div className="text-danger mb-1">
                  {formErrors.minimumStock}
                </div>
              )}
              <input
                type="number"
                name="minimumStock"
                className={`form-control ${formErrors.minimumStock ? "is-invalid" : ""}`}
                value={productForm.minimumStock}
                onChange={handleChange}
              />
            </div>

            {/* Category */}
            <div className="col-md-6">
              <label className="form-label">Category</label>
              {/*Error message for category field */}
              {formErrors.categoryId && (
                <div className="text-danger mb-1">{formErrors.categoryId}</div>
              )}
              <select
                name="categoryId"
                className={`form-select ${formErrors.categoryId ? "is-invalid" : ""}`}
                value={productForm.categoryId}
                onChange={handleChange}
              >
                <option value="">Select category...</option>
                {categories.map((category) => (
                  <option key={category.categoryId} value={category.categoryId}>
                    {category.name}
                  </option>
                ))}
              </select>
              {categoriesError && (
                <div className="text-danger mt-1">{categoriesError}</div>
              )}
              {loadingCategories && <div>Loading categories...</div>}
            </div>

            {/* Supplier */}
            <div className="col-md-6">
              <label className="form-label">Supplier</label>
              {/*Error message for supplier field */}
              {formErrors.supplierId && (
                <div className="text-danger mb-1">{formErrors.supplierId}</div>
              )}
              <select
                name="supplierId"
                className={`form-select ${formErrors.supplierId ? "is-invalid" : ""}`}
                value={productForm.supplierId}
                onChange={handleChange}
              >
                <option value="">Select supplier...</option>
                {suppliers.map((supplier) => (
                  <option key={supplier.supplierId} value={supplier.supplierId}>
                    {supplier.companyName}
                  </option>
                ))}
              </select>
              {suppliersError && (
                <div className="text-danger mt-1">{suppliersError}</div>
              )}
              {loadingSuppliers && <div>Loading suppliers...</div>}
            </div>

            {/* Location */}
            <div className="col-md-6">
              <label className="form-label">Location</label>
              {formErrors.location && (
                <div className="text-danger mb-1">{formErrors.location}</div>
              )}
              <input
                type="text"
                name="location"
                className={`form-control ${formErrors.location ? "is-invalid" : ""}`}
                value={productForm.location}
                onChange={handleChange}
              />
            </div>

            {/* Buttons */}
            <div className="col-12 d-flex gap-2 mt-2">
              <button
                type="submit"
                className="btn btn-primary"
                disabled={productLoading}
              >
                Save
              </button>

              <button
                type="button"
                className="btn btn-outline-secondary"
                onClick={() => (isModal ? onDone?.() : navigate("/products"))}
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
