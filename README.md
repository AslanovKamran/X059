
# Создание собственного самоподписанного сертификата X509

Для начала нужно скачать и установить OpenSSL на свой ПК. Для этого можно перейти по следующей ссылке и скачать подходящий `.exe` файл: [Скачать OpenSSL](https://slproweb.com/products/Win32OpenSSL.html) (мой выбор — Win64 OpenSSL v3.3.2).

Далее в CMD вводим следующую команду для создания сертификата X509, который используется для создания ЭП:

```bash
C:\Users\DOIT\Desktop> openssl req -x509 -days 365 -newkey rsa:2048 -keyout my-key.pem -out my-cert.pem
```

### Что делает эта команда:
- Команда генерирует новый самоподписанный сертификат RSA с длиной ключа 2048 бит и сохраняет его в файле `my-cert.pem`.
- Одновременно создается приватный ключ для сертификата, который сохраняется в файле `my-key.pem`.
- Этот сертификат будет действовать в течение 365 дней, после чего его нужно будет обновить.

Затем будет предложено ввести информацию, такую как:
- **PEM pass phrase**: пароль, который используется для защиты приватного ключа, сохраняемого в формате PEM.
- **Country Name** (например, "US")
- **State or Province Name** (например, "DC")
- **Locality Name** (например, "Washington")
- **Organization Name** (например, "MyCompany")
- **Organizational Unit Name** (например, "IT Department")
- **Common Name** (например, "www.mycompany.com")
- **Email Address** (например, "admin@mycompany.com")

Эта информация будет включена в самоподписанный сертификат.

После этого на рабочем столе появятся два файла:
- **Приватный ключ** будет сохранен в файл с именем `my-key.pem`. Этот файл будет содержать приватный ключ в формате PEM, который используется для хранения криптографических данных.
- **Самоподписанный сертификат** будет сохранен в файл с именем `my-cert.pem`. Этот файл будет содержать публичный сертификат, который можно использовать для обмена с другими системами.

---

## Объединение файлов в один PFX файл

Для объединения и инкапсуляции этих двух файлов в один PFX файл, используем команду:

```bash
C:\Users\DOIT\Desktop> openssl pkcs12 -export -in my-cert.pem -inkey my-key.pem -out doit.test-cert.pfx
```

После ввода пароля для PFX файла на рабочем столе появится файл `doit.test-cert.pfx`, который включает:
- Публичный сертификат
- Приватный ключ

Файл с расширением `.pfx` используется для хранения сертификатов и соответствующих приватных ключей в одном зашифрованном контейнере. Этот файл может быть использован для импорта и экспорта сертификатов с приватными ключами, а также для их передачи между различными системами. Обычно файл `.pfx` зашифрован паролем, который требуется для его открытия и использования, что обеспечивает защиту приватного ключа.

---

## Экспорт публичного ключа

Последний шаг — получение публичного ключа из нашего сертификата, который в дальнейшем будет использоваться для верификации ЭП. Для экспорта публичного ключа воспользуемся следующей командой:

```bash
C:\Users\DOIT\Desktop> pkcs12 -in doit-test-cert.pfx -clcerts -nokeys -out doit-test-cert-public.pem
```

Эта команда извлекает только публичный сертификат из файла `.pfx` и сохраняет его в файл PEM (`doit-test-cert-public.pem`), не извлекая приватный ключ. Публичный сертификат в формате PEM может быть использован для проверки подписи или установления защищенного соединения.

---

### В результате получаем два файла:
- `doit.test-cert.pfx` — сертификат, который используется для подписания документа в программе X509SignTest.
- `doit-test-cert-public.pem` — публичный ключ, который используется для верификации в программе X509ValidateTest.
