import { useState } from 'react'
import { App, Form, Input, Modal } from 'antd'
import { useDepartment } from '../context/DepartmentContext'
import { getErrorMessage } from '../utils/getErrorMessage'

export function DepartmentsModal({ department, onClose }) {
  const [form] = Form.useForm()
  const [saving, setSaving] = useState(false)
  
  const { addDepartment, updateDepartment } = useDepartment()
  const { message } = App.useApp()

  const isEdit = Boolean(department)

  async function handleSubmit(values) {
    setSaving(true)
    try {
      if (isEdit) {
        await updateDepartment({ id: department.id, ...values })
        message.success('Departman güncellendi.')
      } else {
        await addDepartment(values)
        message.success('Departman eklendi.')
      }
      onClose(true)
    } catch (error) {
      message.error(getErrorMessage(error))
    } finally {
      setSaving(false)
    }
  }

return (
    <Modal
      open
      title={isEdit ? 'Departmanı Düzenle' : 'Yeni Departman'}
      okText={isEdit ? 'Kaydet' : 'Ekle'}
      cancelText="Vazgeç"
      confirmLoading={saving}
      onOk={() => form.submit()}
      onCancel={() => onClose(false)}
      destroyOnHidden
    >
      <Form
        form={form}
        layout="vertical"
        initialValues={{ name: department?.name }}
        onFinish={handleSubmit}
      >
        <Form.Item
          name="name"
          label="Departman Adı"
          rules={[
            { required: true, whitespace: true, message: 'Departman adı zorunludur.' },
            { min: 2, max: 100, message: 'Departman adı 2-100 karakter olmalıdır.' },
          ]}
        >
          <Input placeholder="Örn. İnsan Kaynakları" autoFocus />
        </Form.Item>
      </Form>
    </Modal>
  )
}