import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import useSuppliers from "../../hooks/useSuppliers";
import PageTitle from "../../components/layout/PageTitle";
import { validate } from "../../utils/validator";
export default function SupplierCreatePage({
  isModal = false,
  supplierId = null,
  onDone,
}) {
  // Get supplier ID from URL params to determine if it's edit mode
  const params = useParams();
  const navigate = useNavigate();

  const effectiveId = supplierId ?? params.Id;
  const isEditMode = Boolean(effectiveId);

  // State for supplier data (placeholder, to be implemented)
  const [supplierLoading, setSupplierLoading] = useState(false);
  const [supplierError, setSupplierError] = useState(null);

  // Form state (placeholder structure)
  const [supplierFormData, setSupplierFormData] = useState({
    companyName: "",
    email: "",
    phoneNumber: "",
    website: "",
    contactPerson: "",
    billingAddress: {
      streetAddress: "",
      city: "",
      postalCode: "",
      country: "",
    },
    shippingAddress: {
      streetAddress: "",
      city: "",
      postalCode: "",
      country: "",
    },
  });

  // Utilize the custom hook to manage suppliers
  const { getSupplierById, createSupplier, updateSupplier } = useSuppliers();

  const emptyErrors = {
    companyName: "",
    contactPerson: "",
    email: "",
    phoneNumber: "",
    website: "",
    billingAddress: {
      streetAddress: "",
      city: "",
      postalCode: "",
      country: "",
    },
    shippingAddress: {
      streetAddress: "",
      city: "",
      postalCode: "",
      country: "",
    },
  };
  // State for form errors
  const [formErrors, setFormErrors] = useState(emptyErrors);

  const supplierSchema = {
    companyName: [["required", "Company Name is required."]],
    contactPerson: [["required", "Contact Person is required."]],
    email: [
      ["required", "Email is required."],
      ["email", "Invalid email format."],
    ],
    phoneNumber: [
      ["required", "Phone Number is required."],
      ["phone", "Invalid phone number format."],
    ],
    website: [
      ["required", "Website is required."],
      ["url", "Invalid URL format."],
    ],
    "billingAddress.streetAddress": [
      ["required", "Billing Street Address is required."],
    ],
    "billingAddress.city": [["required", "Billing City is required."]],
    "billingAddress.postalCode": [
      ["required", "Billing Postal Code is required."],
    ],
    "billingAddress.country": [["required", "Billing Country is required."]],
    "shippingAddress.streetAddress": [
      ["required", "Shipping Street Address is required."],
    ],
    "shippingAddress.city": [["required", "Shipping City is required."]],
    "shippingAddress.postalCode": [
      ["required", "Shipping Postal Code is required."],
    ],
    "shippingAddress.country": [["required", "Shipping Country is required."]],
  };

  const handleSave = async (e) => {
    e.preventDefault();
    try {
      setSupplierLoading(true);
      setSupplierError(null);

      // Validate all fields before submission
      const { errors, hasErrors } = validate(supplierFormData, supplierSchema);
      setFormErrors({ ...emptyErrors, ...errors });
      if (hasErrors) return;

      // Prepare payload for API
      const payload = {
        companyName: supplierFormData.companyName,
        contactPerson: supplierFormData.contactPerson,
        email: supplierFormData.email,
        phoneNumber: supplierFormData.phoneNumber,
        website: supplierFormData.website,
        billingAddress: supplierFormData.billingAddress,
        shippingAddress: supplierFormData.shippingAddress,
      };
      // Call create or update based on mode
      if (isEditMode) {
        await updateSupplier(effectiveId, payload);
      } else {
        await createSupplier(payload);
      }

      // Navigate back to suppliers list after saving
      if (isModal && onDone) onDone();
      navigate("/suppliers");
    } catch (error) {
      // Handle and display error
      setSupplierError(error.message ?? "Save failed");
    } finally {
      // Reset loading state
      setSupplierLoading(false);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;

    setSupplierFormData((prev) => {
      const next = { ...prev, [name]: value };

      const { errors } = validate(next, { [name]: supplierSchema[name] });

      setFormErrors((prevErrors) => ({
        ...prevErrors,
        [name]: errors[name] ?? "",
      }));

      return next;
    });
  };

  const handleAddressChange = (section) => (e) => {
    const { name, value } = e.target;
    const key = `${section}.${name}`;

    setSupplierFormData((prev) => {
      const next = {
        ...prev,
        [section]: { ...prev[section], [name]: value },
      };

      const { errors } = validate(next, { [key]: supplierSchema[key] });

      setFormErrors((prevErrors) => ({
        ...prevErrors,
        [section]: {
          ...prevErrors[section],
          [name]: errors[section]?.[name] ?? "",
        },
      }));

      return next;
    });
  };

  useEffect(() => {
    if (!isEditMode) return;

    setSupplierLoading(true);
    getSupplierById(effectiveId)
      .then((data) => {
        setSupplierFormData({
          companyName: data.companyName || "",
          email: data.email || "",
          phoneNumber: data.phoneNumber || "",
          website: data.website || "",
          contactPerson: data.contactPerson || "",
          billingAddress: {
            streetAddress: data.billingAddress?.streetAddress || "",
            city: data.billingAddress?.city || "",
            postalCode: data.billingAddress?.postalCode || "",
            country: data.billingAddress?.country || "",
          },
          shippingAddress: {
            streetAddress: data.shippingAddress?.streetAddress || "",
            city: data.shippingAddress?.city || "",
            postalCode: data.shippingAddress?.postalCode || "",
            country: data.shippingAddress?.country || "",
          },
        });
      })
      .catch((error) => setSupplierError(error.message ?? "Load failed"))
      .finally(() => setSupplierLoading(false));
  }, [effectiveId, isEditMode, getSupplierById]);

  return (
    <div className="container-fluid p-3">
      {/* Page header */}
      <PageTitle title={isEditMode ? "Edit Supplier" : "Create Supplier"} />

      {/* Error and loading messages */}
      {supplierError && (
        <div className="alert alert-danger">{supplierError}</div>
      )}
      {supplierLoading && <div>Loading supplier data...</div>}

      <div className="card shadow-sm">
        <div className="card-body">
          {/* Supplier form goes here */}
          <form className="row g-3" onSubmit={handleSave}>
            {/* Supplier Name */}
            <div className="col-md-6">
              <label className="form-label">Company Name</label>
              {/*Error message for price field */}
              {formErrors.companyName && (
                <div className="text-danger mb-1">{formErrors.companyName}</div>
              )}
              <input
                type="text"
                name="companyName"
                className={`form-control ${formErrors.companyName ? "is-invalid" : ""}`}
                value={supplierFormData.companyName}
                onChange={handleChange}
              />
            </div>
            {/* Supplier Contact */}
            <div className="col-md-6">
              <label className="form-label">Contact Name</label>
              {formErrors.contactPerson && (
                <div className="text-danger mb-1">
                  {formErrors.contactPerson}
                </div>
              )}
              <input
                type="text"
                name="contactPerson"
                className={`form-control ${formErrors.contactPerson ? "is-invalid" : ""}`}
                value={supplierFormData.contactPerson}
                onChange={handleChange}
              />
            </div>

            {/* Supplier Email */}
            <div className="col-md-6">
              <label className="form-label">Email</label>
              {formErrors.email && (
                <div className="text-danger mb-1">{formErrors.email}</div>
              )}
              <input
                type="email"
                name="email"
                className={`form-control ${formErrors.email ? "is-invalid" : ""}`}
                value={supplierFormData.email}
                onChange={handleChange}
              />
            </div>

            {/* Supplier Phone */}
            <div className="col-md-6">
              <label className="form-label">Phone</label>
              {formErrors.phoneNumber && (
                <div className="text-danger mb-1">{formErrors.phoneNumber}</div>
              )}
              <input
                type="tel"
                name="phoneNumber"
                className={`form-control ${formErrors.phoneNumber ? "is-invalid" : ""}`}
                value={supplierFormData.phoneNumber}
                onChange={handleChange}
              />
            </div>

            {/* Supplier Website */}
            <div className="col-md-6">
              <label className="form-label">Website</label>
              {formErrors.website && (
                <div className="text-danger mb-1">{formErrors.website}</div>
              )}
              <input
                type="url"
                name="website"
                className={`form-control ${formErrors.website ? "is-invalid" : ""}`}
                value={supplierFormData.website}
                onChange={handleChange}
              />
            </div>

            <div className="row">
              {/* Billing Address */}
              <div className="col-md-6">
                <h5 className="mt-3">Billing Address</h5>

                <div className="row g-3">
                  <div className="col-md-6">
                    <label className="form-label">Street</label>
                    {formErrors.billingAddress.streetAddress && (
                      <div className="text-danger mb-1">
                        {formErrors.billingAddress.streetAddress}
                      </div>
                    )}
                    <input
                      type="text"
                      name="streetAddress"
                      className={`form-control w-100 ${formErrors.billingAddress.streetAddress ? "is-invalid" : ""}`}
                      value={supplierFormData.billingAddress.streetAddress}
                      onChange={handleAddressChange("billingAddress")}
                    />
                  </div>

                  <div className="col-md-6">
                    <label className="form-label">City</label>
                    {formErrors.billingAddress.city && (
                      <div className="text-danger mb-1">
                        {formErrors.billingAddress.city}
                      </div>
                    )}
                    <input
                      type="text"
                      name="city"
                      className={`form-control w-100 ${formErrors.billingAddress.city ? "is-invalid" : ""}`}
                      value={supplierFormData.billingAddress.city}
                      onChange={handleAddressChange("billingAddress")}
                    />
                  </div>

                  <div className="col-md-6">
                    <label className="form-label">Postal Code</label>
                    {formErrors.billingAddress.postalCode && (
                      <div className="text-danger mb-1">
                        {formErrors.billingAddress.postalCode}
                      </div>
                    )}
                    <input
                      type="text"
                      name="postalCode"
                      className={`form-control w-100 ${formErrors.billingAddress.postalCode ? "is-invalid" : ""}`}
                      value={supplierFormData.billingAddress.postalCode}
                      onChange={handleAddressChange("billingAddress")}
                    />
                  </div>

                  <div className="col-md-6">
                    <label className="form-label">Country</label>
                    {formErrors.billingAddress.country && (
                      <div className="text-danger mb-1">
                        {formErrors.billingAddress.country}
                      </div>
                    )}
                    <input
                      type="text"
                      name="country"
                      className={`form-control w-100 ${formErrors.billingAddress.country ? "is-invalid" : ""}`}
                      value={supplierFormData.billingAddress.country}
                      onChange={handleAddressChange("billingAddress")}
                    />
                  </div>
                </div>
              </div>

              {/* Shipping Address */}
              <div className="col-md-6">
                <h5 className="mt-3">Shipping Address</h5>

                <div className="row g-3">
                  <div className="col-md-6">
                    <label className="form-label">Street</label>
                    {formErrors.shippingAddress.streetAddress && (
                      <div className="text-danger mb-1">
                        {formErrors.shippingAddress.streetAddress}
                      </div>
                    )}
                    <input
                      type="text"
                      name="streetAddress"
                      className={`form-control w-100 ${formErrors.shippingAddress.streetAddress ? "is-invalid" : ""}`}
                      value={supplierFormData.shippingAddress.streetAddress}
                      onChange={handleAddressChange("shippingAddress")}
                    />
                  </div>

                  <div className="col-md-6">
                    <label className="form-label">City</label>
                    {formErrors.shippingAddress.city && (
                      <div className="text-danger mb-1">
                        {formErrors.shippingAddress.city}
                      </div>
                    )}
                    <input
                      type="text"
                      name="city"
                      className={`form-control w-100 ${formErrors.shippingAddress.city ? "is-invalid" : ""}`}
                      value={supplierFormData.shippingAddress.city}
                      onChange={handleAddressChange("shippingAddress")}
                    />
                  </div>

                  <div className="col-md-6">
                    <label className="form-label">Postal Code</label>
                    {formErrors.shippingAddress.postalCode && (
                      <div className="text-danger mb-1">
                        {formErrors.shippingAddress.postalCode}
                      </div>
                    )}
                    <input
                      type="text"
                      name="postalCode"
                      className={`form-control w-100 ${formErrors.shippingAddress.postalCode ? "is-invalid" : ""}`}
                      value={supplierFormData.shippingAddress.postalCode}
                      onChange={handleAddressChange("shippingAddress")}
                    />
                  </div>

                  <div className="col-md-6">
                    <label className="form-label">Country</label>
                    {formErrors.shippingAddress.country && (
                      <div className="text-danger mb-1">
                        {formErrors.shippingAddress.country}
                      </div>
                    )}
                    <input
                      type="text"
                      name="country"
                      className={`form-control w-100 ${formErrors.shippingAddress.country ? "is-invalid" : ""}`}
                      value={supplierFormData.shippingAddress.country}
                      onChange={handleAddressChange("shippingAddress")}
                    />
                  </div>
                </div>
              </div>
            </div>

            {/* Form actions */}
            <div className="col-12 d-flex gap-2 mt-2">
              <button
                type="submit"
                className="btn btn-primary"
                disabled={supplierLoading}
              >
                Save
              </button>

              <button
                type="button"
                className="btn btn-outline-secondary"
                onClick={() => (isModal ? onDone?.() : navigate("/suppliers"))}
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
