import React from "react";

function EditButton({ ...props }) {
  return (
    <button
      type="button"
      className="btn btn-primary btn-sm"
      {...props}
    >
      Edit
    </button>
  );
}

export default EditButton;
