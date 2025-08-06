import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

type FormData = {
  username: string;
  email: string;
  password: string;
};

const SignUp: React.FC = () => {
  const [formData, setFormData] = useState<FormData>({
    username: "",
    email: "",
    password: "",
  });
  const navigate = useNavigate();
  const [success, setSuccess] = useState<boolean>(false);
  const [error, setError] = useState<boolean>(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { id, value } = e.target;
    setFormData((prev) => ({ ...prev, [id]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      const res = await fetch("api/auth/register", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(formData),
      });

      if (!res.ok) {
        setError(true);
        return;
      }

      setSuccess(true);
      setError(false);

      setFormData({
        username: "",
        email: "",
        password: "",
      });

      window.setTimeout(() => {
        navigate("/login");
      }, 1500);
    } catch {
      setError(true);
    }
  };

  return (
    <>
      <h2 className="text-2xl font-semibold mx-auto mt-10">Sign Up:</h2>

      <form
        className="flex flex-col w-full md:max-w-md mx-auto gap-y-4 mt-5"
        onSubmit={handleSubmit}
      >
        <div className="flex flex-col mx-5 md:mx-0">
          <label htmlFor="username" className="mb-1 font-medium">
            Name
          </label>
          <input
            type="text"
            id="username"
            className="border border-gray-300 rounded py-1"
            value={formData.username}
            onChange={handleChange}
            required
            disabled={success}
          />
        </div>

        <div className="flex flex-col mx-5 md:mx-0">
          <label htmlFor="email" className="mb-1 font-medium">
            Email
          </label>
          <input
            type="email"
            id="email"
            className="border border-gray-300 rounded py-1"
            value={formData.email}
            onChange={handleChange}
            required
            autoComplete="email"
            disabled={success}
          />
        </div>

        <div className="flex flex-col mx-5 md:mx-0">
          <label htmlFor="password" className="mb-1 font-medium">
            Password
          </label>
          <input
            type="password"
            id="password"
            className="border border-gray-300 rounded py-1"
            value={formData.password}
            onChange={handleChange}
            required
            autoComplete="new-password"
            disabled={success}
          />
        </div>

        <button
          type="submit"
          className={`bg-primary font-medium rounded text-gray-200 hover:bg-primary-light transition py-2 cursor-pointer  mx-5 md:mx-0 ${
            success ? "cursor-default pointer-events-none bg-primary-light" : ""
          }`}
          disabled={success}
        >
          Sign Up
        </button>

        {success && (
          <p className="text-center text-sm text-green-500">
            Successfully signed up! Redirecting...
          </p>
        )}

        {error && (
          <p className="text-center text-sm text-red-500">
            Something went wrong. Please try again later.
          </p>
        )}
      </form>
    </>
  );
};

export default SignUp;
