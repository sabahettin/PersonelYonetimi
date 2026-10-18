
import {
  DeleteOutlined,
  EditOutlined,
  MoreOutlined,
} from '@ant-design/icons';

import {
  Button,
  Dropdown,
  Popconfirm,
} from 'antd';

export function RowActions({
  onEdit,
  onDelete,
  deleteTitle = 'Kaydı Sil',
  deleteDescription = 'Bu kaydı silmek istediğinize emin misiniz?',
  deleteOkText = 'Evet, Sil',
  deleteCancelText = 'İptal',
}) {
  const items = [
    {
      key: 'edit',
      label: 'Düzenle',
      icon: <EditOutlined />,
    },
    {
      key: 'delete',
      label: (
        <Popconfirm
          title={deleteTitle}
          description={deleteDescription}
          onConfirm={onDelete}
          okText={deleteOkText}
          cancelText={deleteCancelText}
          okButtonProps={{ danger: true }}
        >
          <span
            style={{
              display: 'block',
              margin: '-5px -12px',
              padding: '5px 12px',
            }}
          >
            Sil
          </span>
        </Popconfirm>
      ),
      icon: <DeleteOutlined />,
      danger: true,
    },
  ];

  function handleClick({ key, domEvent }) {
    if (key === 'edit') {
      onEdit?.();
    }

    if (key === 'delete') {
      domEvent.stopPropagation();
    }
  }

  return (
    <Dropdown
      trigger={['click']}
      placement="bottomRight"
      menu={{
        items,
        onClick: handleClick,
      }}
    >
      <Button
        type="text"
        icon={<MoreOutlined />}
        aria-label="İşlemler"
      />
    </Dropdown>
  );
}
