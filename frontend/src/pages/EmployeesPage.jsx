import { useState, useEffect } from 'react';
import { App, Table, Card, Button } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { useEmployee } from '../context/EmployeeContext';
import { EmployeesModal } from './EmployeesModal';
import { RowActions } from '../components/RowActions';
import { getErrorMessage } from '../utils/getErrorMessage';

export default function EmployeesPage() {
    const {
        employees,
        loading,
        removeEmployee,
        listEmployees
    } = useEmployee();
    const { message } = App.useApp();

    const [editing, setEditing] = useState(undefined);

    useEffect(() => {
        listEmployees();
    }, []);

    function handleFormClose() {
        setEditing(undefined);
    }

    async function handleDelete(id) {
        try {
            await removeEmployee(id);
            message.success('Çalışan silindi.');
        } catch (error) {
            message.error(getErrorMessage(error, 'Silme işlemi başarısız.'));
        }
    }

    const columns = [
        {
            title: 'Adı',
            dataIndex: 'name',
            key: 'name',
        },
        {
            title: 'Soyadı',
            dataIndex: 'surname',
            key: 'surname',
        },
        {
            title: 'E-Posta',
            dataIndex: 'email',
            key: 'email',
        },
        {
            title: 'Departman',
            dataIndex: 'departmentName',
            key: 'departmentName',
        },
{
  title: 'İşlemler',
  key: 'actions',
  width: 100,
  align: 'center',
  render: (_, record) => (
    <RowActions
      onEdit={() => setEditing(record.id)}
      onDelete={() => handleDelete(record.id)}
      deleteTitle="Çalışanı Sil"
      deleteDescription={`"${record.name} ${record.surname}" çalışanını silmek istediğinize emin misiniz?`}
    />
  ),
},
    ];

    return (
        <Card
            title="Çalışanlar"
            extra={
                <Button
                    type="primary"
                    icon={<PlusOutlined />}
                    onClick={() => setEditing(null)}
                >
                    Yeni Çalışan Ekle
                </Button>
            }
        >
            <Table
                columns={columns}
                dataSource={Array.isArray(employees) ? employees : []}
                loading={loading}
                rowKey="id"
                locale={{ emptyText: 'Henüz kayıtlı çalışan bulunmuyor.' }}
            />

            {editing !== undefined && (
                <EmployeesModal employeeId={editing} onClose={handleFormClose} />
            )}
        </Card>
    );
}