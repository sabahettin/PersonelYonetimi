import Sidebar from './layout/sidebar/sidebar';
import { DepartmentProvider } from './context/DepartmentContext';
import { EmployeeProvider } from './context/EmployeeContext';
import { App as AntApp } from 'antd';

function App() {
  return (
    <AntApp>
      <DepartmentProvider>
        <EmployeeProvider>
          <Sidebar />
        </EmployeeProvider>
      </DepartmentProvider>
    </AntApp>
  );
}

export default App;