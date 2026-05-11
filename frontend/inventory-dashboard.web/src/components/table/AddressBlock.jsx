import React from "react";
import DetailField from "./DetailField";

export default function AddressBlock({ address }) {
  if (!address) {
    return <div className="text-muted">No address available</div>;
  }

  return (
    <dl className="row mb-0">
      <DetailField label="Street">{address.streetAddress}</DetailField>

      <DetailField label="City">{address.city}</DetailField>

      <DetailField label="Postal Code">{address.postalCode}</DetailField>

      <DetailField label="Country">{address.country}</DetailField>
    </dl>
  );
}
