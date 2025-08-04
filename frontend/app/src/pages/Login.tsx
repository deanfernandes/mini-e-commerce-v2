import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";

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
  const [error, setError] = useState<string | null>(null);
  const { login } = useAuth();

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
        if (res.status === 401) {
          const text = await res.text();
          setError(text || "Unauthorized access.");
        } else {
          setError("Something went wrong. Please try again later.");
        }
        return;
      }

      setError(null);

      setFormData({
        email: "",
        password: "",
      });

      const data = await res.json();
      login(data.token);
    } catch {
      setError("Something went wrong. Please try again later.");
    }
  };

  return (
    <>
      <h2 className="font-bold text-2xl mx-auto mt-10">Login:</h2>

      <form
        className="flex flex-col w-full md:max-w-md mx-auto gap-y-4 mt-5"
        onSubmit={handleSubmit}
      >
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
          />
        </div>

        <button
          type="submit"
          className="bg-primary font-medium rounded text-white hover:bg-primary-light transition py-2 cursor-pointer mx-5 md:mx-0"
        >
          Login
        </button>

        {error && <p className="text-center text-sm text-red-500">{error}</p>}
      </form>
    </>
  );
};

export default Login;
