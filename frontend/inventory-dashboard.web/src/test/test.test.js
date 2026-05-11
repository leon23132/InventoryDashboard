import { describe, it, expect } from "vitest";

describe("Sample Test", () => {
  it("should add two numbers correctly", () => {
    const sum = (a, b) => a + b;
    expect(sum(2, 3)).toBe(5);
  });
});
