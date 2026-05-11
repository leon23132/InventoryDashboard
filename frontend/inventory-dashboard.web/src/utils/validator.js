const rules = {
  required: (value, message = "This field is required.") =>
    value !== null && value !== undefined && value.toString().trim() !== ""
      ? ""
      : message,

  email: (value, message = "Invalid email format.") => {
    if (!value) return "";
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(value) ? "" : message;
  },

  phone: (value, message = "Invalid phone number.") => {
    if (!value) return "";
    const cleaned = value.replace(/[^\d+]/g, "");
    const regex = /^\+?[1-9]\d{1,14}$/;
    return regex.test(cleaned) ? "" : message;
  },

  minItems: (
    value,
    options = { length: 1, message: "Select at least 1 item(s)." },
  ) => {
    const length = typeof options === "number" ? options : options.length;
    const message =
      typeof options === "number"
        ? `Select at least ${length} item(s).`
        : (options.message ?? `Select at least ${length} item(s).`);

    return value && value.length >= length ? "" : message;
  },

  url: (value, message = "Invalid URL.") => {
    if (!value) return "";
    const withProtocol = /^https?:\/\//i.test(value)
      ? value
      : `https://${value}`;
    try {
      new URL(withProtocol);
      return "";
    } catch {
      return message;
    }
  },

  minLength: (
    value,
    options = { length: 1, message: "Must be at least 1 characters." },
  ) => {
    const length = typeof options === "number" ? options : options.length;
    const message =
      typeof options === "number"
        ? `Must be at least ${length} characters.`
        : (options.message ?? `Must be at least ${length} characters.`);

    return value && value.length >= length ? "" : message;
  },

  maxLength: (
    value,
    options = { length: 100, message: "Text is too long." },
  ) => {
    const length = typeof options === "number" ? options : options.length;
    const message =
      typeof options === "number"
        ? `Must be at most ${length} characters.`
        : (options.message ?? `Must be at most ${length} characters.`);

    if (value === null || value === undefined) return "";
    return value.toString().length <= length ? "" : message;
  },

  number: (value, message = "Must be a valid number.") => {
    if (value === "" || value === null || value === undefined) return "";
    return Number.isNaN(Number(value)) ? message : "";
  },

  minNumber: (value, options = { min: 0, message: "Value is too small." }) => {
    const min = typeof options === "number" ? options : options.min;
    const message =
      typeof options === "number"
        ? `Must be at least ${min}.`
        : (options.message ?? `Must be at least ${min}.`);

    if (value === "" || value === null || value === undefined) return "";
    if (Number.isNaN(Number(value))) return "Must be a valid number.";
    return Number(value) >= min ? "" : message;
  },

  greaterThan: (
    value,
    options = { min: 0, message: "Value must be greater." },
  ) => {
    const min = typeof options === "number" ? options : options.min;
    const message =
      typeof options === "number"
        ? `Must be greater than ${min}.`
        : (options.message ?? `Must be greater than ${min}.`);

    if (value === "" || value === null || value === undefined) return "";
    if (Number.isNaN(Number(value))) return "Must be a valid number.";
    return Number(value) > min ? "" : message;
  },

  integer: (value, message = "Must be a whole number.") => {
    if (value === "" || value === null || value === undefined) return "";
    return Number.isInteger(Number(value)) ? "" : message;
  },
};

function getValueByPath(obj, path) {
  return path.split(".").reduce((acc, key) => acc?.[key], obj);
}

function setValueByPath(obj, path, value) {
  const keys = path.split(".");
  let current = obj;

  for (let i = 0; i < keys.length - 1; i++) {
    const key = keys[i];

    if (!current[key] || typeof current[key] !== "object") {
      current[key] = {};
    }

    current = current[key];
  }

  current[keys[keys.length - 1]] = value;
}

export function validate(data, schema) {
  const errors = {};
  let hasErrors = false;

  for (const field in schema) {
    const fieldRules = schema[field];
    const value = getValueByPath(data, field);

    for (const rule of fieldRules) {
      let msg = "";

      if (Array.isArray(rule)) {
        const [ruleName, param] = rule;
        msg = rules[ruleName]?.(value, param) ?? "";
      } else {
        msg = rules[rule]?.(value) ?? "";
      }

      if (msg) {
        setValueByPath(errors, field, msg);
        hasErrors = true;
        break;
      }
    }
  }

  return { errors, hasErrors };
}
