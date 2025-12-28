# Домашнее задание 4 по КПО

## Структура
4 микросервиса:
- apigateway (порт 5050) - маршрутизация заказов
- orders - обработка заказов
- payments - отвечает за оплату
- frontend (порт 5151) - отображение для удобного использования API

## Как запустить
- `docker-compose up`
- `localhost:5050/swagger` - тут лежит **Swagger**
- `localhost:5151` - тут лежит **frontend** (НА НЕМ МОЖНО ПОТЕСТИТЬ)
- `localhost:5050` - тут само API

## API
- GET `/user_id` - генерирует рандомный id чтобы его потом использовать (хз зачем мне удобно было тестить с этой штукой)
- POST `/create_account?user_id=<>` - создает кошелек пользователю
- GET `/balance?user_id=<>` - получает баланс пользователя
- POST `/add_money?user_id=<>&amount=<>` - добавляет денег на кошелек
- GET `/orders?user_id=<>` - получает список заказов пользователя
- GET `/status?order_id=<>` - получает статус заказа (строкой)
- POST `/create_order?user_id=<>&amount=<>&description=<>` - создает заказ и возвращает его id

## Что использовано?
- RabbitMQ
- PostgreSQL
- C# WebAPI
