import { useState } from 'react';
import { ApartmentOutlined, TeamOutlined } from '@ant-design/icons';
import { Layout, Menu } from 'antd';
import DepartmentsPage from '../../pages/DepartmentsPage.jsx';
import EmployeesPage from '../../pages/EmployeesPage.jsx';

const menuItems = [
  { key: 'employees', label: 'Personeller', icon: <TeamOutlined /> },
  { key: 'departments', label: 'Departmanlar', icon: <ApartmentOutlined /> },
];

function Sidebar() {
  const [page, setPage] = useState('employees');

  const renderContent = () => {
    switch (page) {
      case 'departments':
        return <DepartmentsPage />;
      case 'employees':
      default:
        return <EmployeesPage />;
    }
  };

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Layout.Sider theme="dark">
        <div style={{ height: 32, margin: 16, color: '#fff', fontWeight: 'bold', textAlign: 'center' }}>
          YÖNETİM PANELİ
        </div>
        <Menu
          theme="dark"
          mode="inline"
          items={menuItems}
          selectedKeys={[page]}
          onClick={({ key }) => setPage(key)}
        />
      </Layout.Sider>

      <Layout>
        <Layout.Header style={{ background: '#fff', padding: '0 24px' }}>
          <h2 style={{ margin: 0, lineHeight: '64px' }}>
            {page === 'employees' ? 'Personel Listesi' : 'Departmanlar'}
          </h2>
        </Layout.Header>

        <Layout.Content style={{ margin: '24px', padding: 24, background: '#fff', minHeight: 280 }}>
          {renderContent()}
        </Layout.Content>
      </Layout>
    </Layout>
  );
}

export default Sidebar;