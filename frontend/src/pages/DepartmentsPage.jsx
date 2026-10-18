import { useState, useEffect } from 'react';
import { App, Table, Card, Button } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { useDepartment } from '../context/DepartmentContext';
import { DepartmentsModal } from './DepartmentsModal';
import { RowActions } from '../components/RowActions';
import { getErrorMessage } from '../utils/getErrorMessage';

export default function DepartmentsPage() {
  const {
    departments,
    loading,
    removeDepartment,
    listDepartments,
  } = useDepartment();
  const { message } = App.useApp();

  const [editing, setEditing] = useState(undefined);

  useEffect(() => {
    listDepartments();
  }, []);

  function handleFormClose() {
    setEditing(undefined);
  }

  async function handleDelete(id) {
    try {
      await removeDepartment(id);
      message.success('Departman silindi.');
    } catch (error) {
      message.error(getErrorMessage(error, 'Silme işlemi başarısız.'));
    }
  }

  const columns = [
    {
      title: 'Departman Adı',
      dataIndex: 'name',
      key: 'name',
    },
    {
      title: 'İşlemler',
      key: 'actions',
      width: 100,
      align: 'center',
      render: (_, record) => (
        <RowActions
          onEdit={() => setEditing(record)}
          onDelete={() => handleDelete(record.id)}
          deleteTitle="Departmanı Sil"
          deleteDescription={`"${record.name}" departmanını silmek istediğinize emin misiniz?`}
        />
      ),
    },
  ];

  return (
    <Card
      title="Departmanlar"
      extra={
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => setEditing(null)}
        >
          Yeni Departman Ekle
        </Button>
      }
    >
      <Table
        columns={columns}
        dataSource={Array.isArray(departments) ? departments : []}
        loading={loading}
        rowKey="id"
        locale={{
          emptyText: 'Henüz kayıtlı departman bulunmuyor.',
        }}
      />

      {editing !== undefined && (
        <DepartmentsModal
          department={editing}
          onClose={handleFormClose}
        />
      )}
    </Card>
  );
}
