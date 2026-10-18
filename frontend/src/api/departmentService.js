import  api  from './api'

async function getAll() {
try {
    const response = await api.get('/departments');
    return response.data;
  } catch (error) {
    console.error('Error fetching departments:', error);
    throw error;
  }
}

async function add(department) {
  try {
    const response = await api.post('/departments', department);
    return response.data;
  } catch (error) {
    console.error('Error adding department:', error);
    throw error;
  }
}

async function update(department) {
  try {
    const response = await api.put(`/departments/${department.id}`, department);
    return response.data;
  } catch (error) {
    console.error('Error updating department:', error);
    throw error;
  }
}

async function remove(id) {
  try {
    const response = await api.delete(`/departments/${id}`);
    return response.data;
  } catch (error) {
    console.error('Error removing department:', error);
    throw error;
  }
}

export const departmentService = { getAll, add, update, remove }