import React from "react";
import PageLayout from "../../components/layout/PageLayout";
import PageTitle from "../../components/layout/PageTitle";
import ContentCard from "../../components/layout/ContentCard";
import { useNavigate, useParams } from "react-router-dom";
import useProjects from "../../hooks/useProjects";
import { useState, useEffect } from "react";
import FormInput from "../../components/form/FormInput";
import FormSelect from "../../components/form/FormSelect";
import useProducts from "../../hooks/useProducts";
import FormLayout from "../../components/form/FormLayout";
import ProductPicker from "../../components/form/ProductPicker";
import { validate } from "../../utils/validator";
function ProjectCreatePage({ isModal = false, projectId = null, onDone }) {
  const params = useParams();
  const navigate = useNavigate();

  const effectiveId = projectId ?? params.Id;
  const isEditMode = Boolean(effectiveId);
  const [projectLoading, setProjectLoading] = useState(false);
  const [projectError, setProjectError] = useState(null);

  const [formerror, setFormError] = useState(null);
  const [formErrors, setFormErrors] = useState({});

  // Form state
  const [projectFormData, setProjectFormData] = useState({
    projectName: "",
    description: "",
    products: [], // ← wichtig!
  });

  const { getProjectById, createProject, updateProject } = useProjects();
  const { products, loadingProducts, productsError, loadProducts } =
    useProducts();

  useEffect(() => {
    loadProducts({ search: "" });
  }, [loadProducts]);

  useEffect(() => {
    if (!isEditMode) return;

    setProjectLoading(true);
    getProjectById(effectiveId)
      .then((data) => {
        setProjectFormData({
          projectName: data.projectName,
          description: data.description,
          products:
            data.products?.map((p) => ({
              productId: p.productId,
              quantity: p.quantity,
            })) ?? [],
        });
      })
      .catch((err) => {
        setProjectError(err?.message ?? "Failed to load project");
      })
      .finally(() => {
        setProjectLoading(false);
      });
  }, []);

  const projectSchema = {
    projectName: ["required"],
    description: ["required"],
    products: [["minLength", 1]],
  };

  const handleChange = (e) => {
    const { name, value } = e.target;

    setProjectFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSave = async () => {
    // Validate form data
    const { errors, hasErrors } = validate(projectFormData, projectSchema);
    setFormErrors(errors);
    if (hasErrors) return;

    try {
      setProjectLoading(true);
      setProjectError(null);
      // Prepare payload
      const payload = {
        projectName: projectFormData.projectName,
        description: projectFormData.description || null,
        products: (projectFormData.products ?? []).map((p) => ({
          productId: p.productId,
          quantity: p.quantity ?? 1,
        })),
      };

      if (isEditMode) {
        await updateProject(effectiveId, payload);
      } else {
        await createProject(payload);
      }
      if (isModal) {
        onDone?.();
      }
    } catch (err) {
      setProjectError(err?.message ?? "Save failed");
    } finally {
      setProjectLoading(false);
      navigate("/projects");
    }
  };

  const toggleProduct = (id) => {
    setProjectFormData((prev) => {
      const exists = prev.products.some((p) => p.productId === id);

      return {
        ...prev,
        products: exists
          ? prev.products.filter((p) => p.productId !== id)
          : [...prev.products, { productId: id, quantity: 1 }],
      };
    });
  };

  const changeQuantity = (id, quantity) => {
    setProjectFormData((prev) => ({
      ...prev,
      products: prev.products.map((p) =>
        p.productId === id
          ? { ...p, quantity: Math.max(1, Number(quantity)) }
          : p,
      ),
    }));
  };

  const removeProduct = (id) => {
    setProjectFormData((prev) => ({
      ...prev,
      products: prev.products.filter((p) => p.productId !== id),
    }));
  };

  return (
    <>
      <PageLayout>
        <PageTitle title="Create Project" />
        <ContentCard>
          {projectError && (
            <div className="alert alert-danger mb-3">Error: {projectError}</div>
          )}
          {projectLoading && <div>Loading project data...</div>}

          <FormLayout
            onSubmit={handleSave}
            showActions={true}
            submitLabel={isEditMode ? "Save" : "Save"}
            onCancel={() => (isModal ? onDone?.() : navigate("/projects"))}
          >
            <FormInput
              label="Project Name"
              name="projectName"
              type="text"
              value={projectFormData.projectName}
              error={formErrors.projectName}
              col="col-md-6"
              onChange={handleChange}
            />

            <FormInput
              label="Description"
              name="description"
              type="text"
              value={projectFormData.description}
              error={formErrors.description}
              onChange={handleChange}
              col="col-md-6"
            />
            <ProductPicker
              products={products ?? []}
              selectedIds={projectFormData.products}
              onToggle={toggleProduct}
              onQuantityChange={changeQuantity}
              onRemove={removeProduct}
            />
          </FormLayout>
        </ContentCard>
      </PageLayout>
    </>
  );
}

export default ProjectCreatePage;
