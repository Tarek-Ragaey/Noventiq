# Noventiq Admin Panel

A React-based admin panel for managing users and roles in the Noventiq application.

## Features

- User authentication with JWT tokens
- User management (list, create, delete)
- Role management (list, create, update, delete)
- Role translations support
- Protected routes
- Responsive Material-UI design

## Prerequisites

- Node.js (v14 or higher)
- npm (v6 or higher)

## Setup

1. Clone the repository
2. Navigate to the project directory:
   ```bash
   cd noventiq-admin
   ```
3. Install dependencies:
   ```bash
   npm install
   ```
4. Create a `.env` file in the root directory and add your API URL:
   ```
   REACT_APP_API_URL=https://your-api-url.com/api
   ```

## Running the Application

To start the development server:

```bash
npm start
```

The application will be available at `http://localhost:3000`.

## Build for Production

To create a production build:

```bash
npm run build
```

The build files will be created in the `build` directory.

## Project Structure

```
src/
  ├── components/     # Reusable components
  ├── pages/         # Page components
  ├── services/      # API services
  ├── stores/        # State management
  ├── types/         # TypeScript types
  ├── App.tsx        # Main application component
  └── index.tsx      # Application entry point
```

## Authentication

The application uses JWT tokens for authentication. Tokens are stored in localStorage and automatically refreshed when needed.

## Features

### Users Management
- View list of users with pagination
- Create new users
- Delete users
- View user roles

### Roles Management
- View list of roles with pagination
- Create new roles with translations
- Update role names and translations
- Delete roles

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request 