import React from "react";

function ListToolbar({
  search,
  onSearchChange,
  onSearchSubmit,
  primaryAction,
  count,
  placeholder = "Search...",
  Ad_ButtonLabel = "Add",
  Ad_Action_Click,
    children,
}) {
  return (
    <form
      className="row g-2 align-items-center mb-3"
      onSubmit={(e) => {
        e.preventDefault();
        onSearchSubmit?.();
      }}
    >
      {/* Search */}
      <div className="col-12 col-md-6 col-lg-4 d-flex">
        <div className="input-group input-group-sm">
          <input
            type="text"
            className="form-control"
            placeholder={placeholder}
            value={search}
            onChange={(e) => onSearchChange(e.target.value)}
          />
          <button className="input-group-text" type="submit" title="Search">
            🔍
          </button>
          <button
            className="btn btn-primary btn-sm"
            type="button"
            onClick={Ad_Action_Click}
          >
            {Ad_ButtonLabel}
          </button>
        </div>
      </div>

     {children}
    
    </form>
  );
}

export default ListToolbar;
