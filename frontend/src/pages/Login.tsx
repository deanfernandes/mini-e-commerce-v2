import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

type FormData = {
  email: string;
  password: string;
};

const Login: React.FC = () => {
  const [formData, setFormData] = useState<FormData>({
    email: "",
    password: "",
  });
  const navigate = useNavigate();
  const [error, setError] = useState<boolean>(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { id, value } = e.target;
    setFormData((prev) => ({ ...prev, [id]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      const res = await fetch("api/auth/login", {
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

      setError(false);

      setFormData({
        email: "",
        password: "",
      });

      window.setTimeout(() => {
        navigate("/");
      }, 1500);
    } catch {
      setError(true);
    }
  };

  return (
    <>
      <h2 className="font-bold text-2xl ml-2 mt-2">Login:</h2>

      <form
        className="flex flex-col w-full md:max-w-md mx-auto gap-y-4 mt-5"
        onSubmit={handleSubmit}
      >
        <div className="flex flex-col">
          <label htmlFor="email" className="mb-1 font-medium">
            Email
          </label>
          <input
            type="email"
            id="email"
            className="border border-gray-300 rounded"
            value={formData.email}
            onChange={handleChange}
            required
            autoComplete="email"
          />
        </div>

        <div className="flex flex-col">
          <label htmlFor="password" className="mb-1 font-medium">
            Password
          </label>
          <input
            type="password"
            id="password"
            className="border border-gray-300 rounded"
            value={formData.password}
            onChange={handleChange}
            required
            autoComplete="new-password"
          />
        </div>

        <button
          type="submit"
          className="bg-blue-400 font-medium rounded text-white hover:bg-blue-500 transition py-2 cursor-pointer"
        >
          Login
        </button>

        {error && (
          <p className="text-center text-sm text-red-500">
            Something went wrong. Please try again later.
          </p>
        )}
      </form>
    </>
  );
};

export default Login;
