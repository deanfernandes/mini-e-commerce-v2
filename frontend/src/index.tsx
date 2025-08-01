import React from 'react';
import ReactDOM from 'react-dom/client';
import './style.css';

const App: React.FC = () => (
  <h1>Hello, React + TypeScript + Webpack!</h1>
);

const root = ReactDOM.createRoot(document.getElementById('root')!);
root.render(<App />);