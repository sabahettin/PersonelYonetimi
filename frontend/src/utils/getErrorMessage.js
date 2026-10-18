export function getErrorMessage(error, fallback = 'İşlem başarısız.') {
  if (!error?.response) {
    return 'Sunucuya ulaşılamadı. Backend uygulamasının çalıştığından emin olun.';
  }

  const data = error.response.data;

  if (data?.errors) {
    return Object.values(data.errors).flat().join(' ');
  }

  if (error.response.status === 404) {
    return 'Kayıt bulunamadı. Silinmiş olabilir.';
  }

  return data?.detail || data?.title || fallback;
}
