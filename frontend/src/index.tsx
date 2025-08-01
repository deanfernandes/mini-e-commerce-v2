import React from 'react';
import ReactDOM from 'react-dom/client';
import './style.css';

const App: React.FC = () => (
  <h1 className='font-bold text-blue-600 text-3xl underline'>Hello, React + TypeScript + Webpack + Tailwind CSS!</h1>
);

const root = ReactDOM.createRoot(document.getElementById('root')!);
root.render(<App />);