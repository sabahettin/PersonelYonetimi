import { createContext, useContext, useState, useEffect } from 'react';
import { App } from 'antd';
import { departmentService } from '../api/departmentService';
import { getErrorMessage } from '../utils/getErrorMessage';

const DepartmentContext = createContext(null);

export function DepartmentProvider({ children }) {
  const [departments, setDepartments] = useState([]);
  const [loading, setLoading] = useState(false);
  const { message } = App.useApp();

  const listDepartments = async () => {
    setLoading(true);
    try {
      const data = await departmentService.getAll();
      setDepartments(data);
    } catch (err) {
      console.error('Departments could not be loaded:', err);
      message.error(`Departmanlar yüklenemedi. ${getErrorMessage(err)}`);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    listDepartments();
  }, []);

  const addDepartment = async (departmentData) => {
    setLoading(true);
    try {
      await departmentService.add(departmentData);
      await listDepartments();
    } catch (err) {
      console.error('Department could not be added:', err);
      throw err;
    } finally {
      setLoading(false);
    }
  };

  const updateDepartment = async (departmentData) => {
    setLoading(true);
    try {
      await departmentService.update(departmentData);
      await listDepartments();
    } catch (err) {
      console.error('Department could not be updated:', err);
      throw err;
    } finally {
      setLoading(false);
    }
  };

  const removeDepartment = async (id) => {
    setLoading(true);
    try {
      await departmentService.remove(id);
      await listDepartments();
    } catch (err) {
      console.error('Department could not be deleted:', err);
      throw err;
    } finally {
      setLoading(false);
    }
  };

  const value = {
    departments,
    loading,
    listDepartments,
    addDepartment,
    updateDepartment,
    removeDepartment,
  };

  return (
    <DepartmentContext.Provider value={value}>
      {children}
    </DepartmentContext.Provider>
  );
}

export function useDepartment() {
  const context = useContext(DepartmentContext);
  if (!context) {
    throw new Error('useDepartment must be used within a DepartmentProvider!');
  }
  return context;
}