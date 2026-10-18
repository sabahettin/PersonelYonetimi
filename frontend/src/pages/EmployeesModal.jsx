import { useState, useEffect } from 'react'
import { App, Form, Input, Select, Modal, Spin } from 'antd'
import { useEmployee } from '../context/EmployeeContext'
import { useDepartment } from '../context/DepartmentContext'
import { getErrorMessage } from '../utils/getErrorMessage'

export function EmployeesModal({ employeeId, onClose }) {
    const [form] = Form.useForm()
    const [saving, setSaving] = useState(false)
    const [fetching, setFetching] = useState(false)

    const { getEmployee, addEmployee, updateEmployee } = useEmployee()
    const { departments } = useDepartment()
    const { message } = App.useApp()

    const departmentOptions = (Array.isArray(departments) ? departments : [])
        .map(d => ({ value: d.id, label: d.name }))

    const isEdit = Boolean(employeeId)

    useEffect(() => {
        if (!employeeId) return

        let cancelled = false
        async function loadEmployee() {
            setFetching(true)
            try {
                const employee = await getEmployee(employeeId)
                if (cancelled) return
                form.setFieldsValue({
                    name: employee.name,
                    surname: employee.surname,
                    email: employee.email,
                    departmentId: employee.departmentId,
                })
            } catch (error) {
                if (cancelled) return
                message.error(`Çalışan bilgileri yüklenemedi. ${getErrorMessage(error)}`)
                onClose(false)
            } finally {
                if (!cancelled) setFetching(false)
            }
        }
        loadEmployee()

        return () => { cancelled = true }
    }, [employeeId])

    async function handleSubmit(values) {
        setSaving(true)
        try {
            if (isEdit) {
                await updateEmployee({ id: employeeId, ...values })
                message.success('Çalışan güncellendi.')
            } else {
                await addEmployee(values)
                message.success('Çalışan eklendi.')
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
            title={isEdit ? 'Çalışanı Düzenle' : 'Yeni Çalışan'}
            okText={isEdit ? 'Kaydet' : 'Ekle'}
            cancelText="Vazgeç"
            confirmLoading={saving}
            okButtonProps={{ disabled: fetching }}
            onOk={() => form.submit()}
            onCancel={() => onClose(false)}
            destroyOnHidden
        >
            <Spin spinning={fetching}>
            <Form
                form={form}
                layout="vertical"
                onFinish={handleSubmit}
            >
                <Form.Item
                    name="name"
                    label="Adı"
                    rules={[
                        { required: true, whitespace: true, message: 'Ad zorunludur.' },
                        { min: 2, max: 30, message: 'Ad 2-30 karakter olmalıdır.' },
                    ]}
                >
                    <Input placeholder="Örn. Ahmet" autoFocus />
                </Form.Item>
                <Form.Item
                    name="surname"
                    label="Soyadı"
                    rules={[
                        { required: true, whitespace: true, message: 'Soyad zorunludur.' },
                        { min: 2, max: 30, message: 'Soyad 2-30 karakter olmalıdır.' },
                    ]}
                >
                    <Input placeholder="Örn.doğan" />
                </Form.Item>
                <Form.Item
                    name="email"
                    label="E-Posta"
                    rules={[
                        { required: true, whitespace: true, message: 'E-Posta zorunludur.' },
                        { type: 'email', message: 'Geçerli bir e-posta adresi girin.' },
                        { max: 75, message: 'E-posta en fazla 75 karakter olabilir.' },
                    ]}
                >
                    <Input placeholder="örn. john.doe@example.com" />
                </Form.Item>
                <Form.Item
                    name="departmentId"
                    label="Departman"
                    rules={[
                        { required: true, message: 'Departman zorunludur.' },
                    ]}
                >
                    <Select
                        placeholder="Departman seçiniz"
                        allowClear
                        showSearch
                        optionFilterProp="label"
                        options={departmentOptions}
                        notFoundContent="Önce bir departman ekleyin."
                    />
                </Form.Item>
            </Form>
            </Spin>
        </Modal>
    )
}