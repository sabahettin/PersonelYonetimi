import { createContext, useContext, useState, useEffect } from 'react';
import { App } from 'antd';
import { employeeService } from '../api/employeeService';
import { getErrorMessage } from '../utils/getErrorMessage';

const EmployeeContext = createContext(null);

export function EmployeeProvider({ children }) {
    const [employees, setEmployees] = useState([]);
    const [loading, setLoading] = useState(false);
    const { message } = App.useApp();

    const listEmployees = async () => {
        setLoading(true);
        try {
            const data = await employeeService.getAll();
            setEmployees(data);
        } catch (error) {
            console.error('Error fetching employees:', error);
            message.error(`Personeller yüklenemedi. ${getErrorMessage(error)}`);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        listEmployees();
    }, []);

    const getEmployee = async (id) => {
        try {
            return await employeeService.getById(id);
        } catch (error) {
            console.error('Error fetching employee:', error);
            throw error;
        }
    };

    const addEmployee = async (employeeData) => {
        setLoading(true);
        try {
            await employeeService.add(employeeData);
            await listEmployees();
        } catch (error) {
            console.error('Error adding employee:', error);
            throw error;
        } finally {
            setLoading(false);
        }
    };

    const updateEmployee = async (employeeData) => {
        setLoading(true);
        try {
            await employeeService.update(employeeData);
            await listEmployees();
        } catch (error) {
            console.error('Error updating employee:', error);
            throw error;
        } finally {
            setLoading(false);
        }
    };

    const removeEmployee = async (id) => {
        setLoading(true);
        try {
            await employeeService.remove(id);
            await listEmployees();
        } catch (error) {
            console.error('Error removing employee:', error);
            throw error;
        } finally {
            setLoading(false);
        }
    };

    const value = {
        employees,
        loading,
        listEmployees,
        getEmployee,
        addEmployee,
        updateEmployee,
        removeEmployee
    };

    return (
        <EmployeeContext.Provider value={value}>
            {children}
        </EmployeeContext.Provider>
    );
}

export function useEmployee() {
    const context = useContext(EmployeeContext);
    if (!context) {
        throw new Error('useEmployee must be used within an EmployeeProvider!');
    }
    return context;
}
