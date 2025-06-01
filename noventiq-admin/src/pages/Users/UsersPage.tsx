import React, { useEffect, useState } from 'react';
import {
    Box,
    Button,
    Paper,
    Typography,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    TablePagination,
    Chip,
    IconButton,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    SelectChangeEvent,
} from '@mui/material';
import { Delete as DeleteIcon, Add as AddIcon } from '@mui/icons-material';
import { apiService } from '../../services/api.service';
import { UserListDto, CreateUserModel } from '../../types/api.types';

export const UsersPage: React.FC = () => {
    const [users, setUsers] = useState<UserListDto[]>([]);
    const [totalCount, setTotalCount] = useState(0);
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);
    const [openDialog, setOpenDialog] = useState(false);
    const [newUser, setNewUser] = useState<CreateUserModel>({
        userName: '',
        email: '',
        password: '',
        roles: []
    });
    const [availableRoles, setAvailableRoles] = useState<string[]>([]);

    const fetchUsers = async () => {
        const response = await apiService.getUsers(page + 1, rowsPerPage);
        setUsers(response.users);
        setTotalCount(response.totalCount);
    };

    const fetchRoles = async () => {
        const response = await apiService.getRoles();
        setAvailableRoles(response.roles.map(role => role.name));
    };

    useEffect(() => {
        fetchUsers();
        fetchRoles();
    }, [page, rowsPerPage]);

    const handleChangePage = (event: unknown, newPage: number) => {
        setPage(newPage);
    };

    const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0);
    };

    const handleDeleteUser = async (id: string) => {
        if (window.confirm('Are you sure you want to delete this user?')) {
            await apiService.deleteUser(id);
            fetchUsers();
        }
    };

    const handleCreateUser = async () => {
        await apiService.createUser(newUser);
        setOpenDialog(false);
        setNewUser({
            userName: '',
            email: '',
            password: '',
            roles: []
        });
        fetchUsers();
    };

    const handleRoleChange = (event: SelectChangeEvent<string[]>) => {
        setNewUser({
            ...newUser,
            roles: event.target.value as string[]
        });
    };

    return (
        <Box>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
                <Typography variant="h5">Users</Typography>
                <Button
                    variant="contained"
                    startIcon={<AddIcon />}
                    onClick={() => setOpenDialog(true)}
                >
                    Add User
                </Button>
            </Box>

            <TableContainer component={Paper}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>Username</TableCell>
                            <TableCell>Email</TableCell>
                            <TableCell>Roles</TableCell>
                            <TableCell align="right">Actions</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {users.map((user) => (
                            <TableRow key={user.id}>
                                <TableCell>{user.userName}</TableCell>
                                <TableCell>{user.email}</TableCell>
                                <TableCell>
                                    {user.roles.map((role) => (
                                        <Chip
                                            key={role}
                                            label={role}
                                            size="small"
                                            sx={{ mr: 0.5 }}
                                        />
                                    ))}
                                </TableCell>
                                <TableCell align="right">
                                    <IconButton
                                        color="error"
                                        onClick={() => handleDeleteUser(user.id)}
                                    >
                                        <DeleteIcon />
                                    </IconButton>
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
                <TablePagination
                    component="div"
                    count={totalCount}
                    page={page}
                    onPageChange={handleChangePage}
                    rowsPerPage={rowsPerPage}
                    onRowsPerPageChange={handleChangeRowsPerPage}
                />
            </TableContainer>

            <Dialog open={openDialog} onClose={() => setOpenDialog(false)}>
                <DialogTitle>Create New User</DialogTitle>
                <DialogContent>
                    <TextField
                        autoFocus
                        margin="dense"
                        label="Username"
                        fullWidth
                        value={newUser.userName}
                        onChange={(e) => setNewUser({ ...newUser, userName: e.target.value })}
                    />
                    <TextField
                        margin="dense"
                        label="Email"
                        type="email"
                        fullWidth
                        value={newUser.email}
                        onChange={(e) => setNewUser({ ...newUser, email: e.target.value })}
                    />
                    <TextField
                        margin="dense"
                        label="Password"
                        type="password"
                        fullWidth
                        value={newUser.password}
                        onChange={(e) => setNewUser({ ...newUser, password: e.target.value })}
                    />
                    <FormControl fullWidth sx={{ mt: 1 }}>
                        <InputLabel>Roles</InputLabel>
                        <Select
                            multiple
                            value={newUser.roles || []}
                            onChange={handleRoleChange}
                            label="Roles"
                        >
                            {availableRoles.map((role) => (
                                <MenuItem key={role} value={role}>
                                    {role}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setOpenDialog(false)}>Cancel</Button>
                    <Button onClick={handleCreateUser} variant="contained">
                        Create
                    </Button>
                </DialogActions>
            </Dialog>
        </Box>
    );
}; 