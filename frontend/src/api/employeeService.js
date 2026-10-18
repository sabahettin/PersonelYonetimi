import api from './api'

async function getAll() {
  try {
    const response = await api.get('/employees');
    return response.data;
  } catch (error) {
    console.error('Error fetching employees:', error);
    throw error;
  }
}

async function getById(id) {
  try {
    const response = await api.get(`/employees/${id}`);
    return response.data;
  } catch (error) {
    console.error('Error fetching employee:', error);
    throw error;
  }
}

async function add(employee) {
  try {
    const response = await api.post('/employees', employee);
    return response.data;
  } catch (error) {
    console.error('Error adding employee:', error);
    throw error;
  }
}

async function update(employee) {
  try {
    const response = await api.put(`/employees/${employee.id}`, employee);
    return response.data;
  } catch (error) {
    console.error('Error updating employee:', error);
    throw error;
  }
}

async function remove(id) {
  try {
    const response = await api.delete(`/employees/${id}`);
    return response.data;
  } catch (error) {
    console.error('Error removing employee:', error);
    throw error;
  }
}

export const employeeService = { getAll, getById, add, update, remove }