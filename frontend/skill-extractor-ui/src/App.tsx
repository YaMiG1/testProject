import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import MainLayout from './layouts/MainLayout';
import AdminLayout from './layouts/AdminLayout.tsx';
import Home from './pages/Home';
import NewCV from './pages/NewCV.tsx';
import Employees from './pages/Employees.tsx';
import EmployeeDetails from './pages/EmployeeDetails.tsx';
import AdminSkills from './pages/AdminSkills.tsx';

function App() {
  return (
    <Router>
      <Routes>
        {/* Main Layout Routes */}
        <Route element={<MainLayout />}>
          <Route path="/" element={<Home />} />
          <Route path="/new" element={<NewCV />} />
          <Route path="/employees" element={<Employees />} />
          <Route path="/employees/:id" element={<EmployeeDetails />} />
        </Route>

        {/* Admin Layout Routes */}
        <Route element={<AdminLayout />}>
          <Route path="/admin/skills" element={<AdminSkills />} />
        </Route>
      </Routes>
    </Router>
  );
}

export default App;
