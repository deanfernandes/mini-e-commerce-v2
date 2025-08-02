import React from "react";
import logo from "../assets/logo.jpg";
import { useState } from "react";

const Header: React.FC = () => {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <header className="flex items-center justify-between bg-blue-400 text-white py-4 px-2 shadow-md relative">
      <div className="flex items-center ml-3 space-x-4">
        <a href="/">
          <img src={logo} alt="Logo" className="w-16 h-16" />
        </a>
        <h1 className="font-bold text-3xl hidden md:block">title</h1>
      </div>
      <nav className="hidden md:flex mr-3 space-x-4">
        <a
          href="/login"
          className="px-4 py-2 bg-amber-300 text-black hover:bg-amber-400 cursor-pointer"
        >
          Login
        </a>
        <a
          href="/signup"
          className="px-4 py-2 bg-amber-300 text-black hover:bg-amber-400 cursor-pointer"
        >
          Sign Up
        </a>
      </nav>

      <button className="md:hidden mr-3" onClick={() => setIsOpen(!isOpen)}>
        <svg
          className="w-6 h-6 text-black"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
          xmlns="http://www.w3.org/2000/svg"
        >
          {isOpen ? (
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M6 18L18 6M6 6l12 12"
            />
          ) : (
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M4 6h16M4 12h16M4 18h16"
            />
          )}
        </svg>
      </button>
      {isOpen && (
        <nav className="flex flex-col absolute top-full right-0 md:hidden">
          <a
            href="#/login"
            className="px-4 py-2 bg-amber-300 text-black hover:bg-amber-400 cursor-pointer text-center"
            onClick={() => setIsOpen(false)}
          >
            Login
          </a>
          <a
            href="#/signup"
            className="px-4 py-2 bg-amber-300 text-black hover:bg-amber-400 cursor-pointer text-center"
            onClick={() => setIsOpen(false)}
          >
            Sign Up
          </a>
        </nav>
      )}
    </header>
  );
};
export default Header;
