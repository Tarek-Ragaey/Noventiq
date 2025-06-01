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
    IconButton,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField,
} from '@mui/material';
import { Delete as DeleteIcon, Add as AddIcon, Edit as EditIcon } from '@mui/icons-material';
import { apiService } from '../../services/api.service';
import { RoleWithTranslationDto, RoleModel, RoleUpdateModel } from '../../types/api.types';

export const RolesPage: React.FC = () => {
    const [roles, setRoles] = useState<RoleWithTranslationDto[]>([]);
    const [totalCount, setTotalCount] = useState(0);
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);
    const [openDialog, setOpenDialog] = useState(false);
    const [editMode, setEditMode] = useState(false);
    const [currentRole, setCurrentRole] = useState<string>('');
    const [roleData, setRoleData] = useState<RoleModel>({
        name: '',
        translations: {
            en: '',
            ar: ''
        }
    });

    const fetchRoles = async () => {
        const response = await apiService.getRoles(page + 1, rowsPerPage);
        setRoles(response.roles);
        setTotalCount(response.totalCount);
    };

    useEffect(() => {
        fetchRoles();
    }, [page, rowsPerPage]);

    const handleChangePage = (event: unknown, newPage: number) => {
        setPage(newPage);
    };

    const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0);
    };

    const handleDeleteRole = async (name: string) => {
        if (window.confirm('Are you sure you want to delete this role?')) {
            await apiService.deleteRole(name);
            fetchRoles();
        }
    };

    const handleEditRole = (role: RoleWithTranslationDto) => {
        setEditMode(true);
        setCurrentRole(role.name);
        setRoleData({
            name: role.name,
            translations: {
                en: role.name,
                ar: role.translatedName
            }
        });
        setOpenDialog(true);
    };

    const handleCreateOrUpdateRole = async () => {
        if (editMode) {
            await apiService.updateRole(currentRole, {
                newName: roleData.name,
                translations: roleData.translations
            });
        } else {
            await apiService.createRole(roleData);
        }
        setOpenDialog(false);
        setRoleData({
            name: '',
            translations: {
                en: '',
                ar: ''
            }
        });
        setEditMode(false);
        fetchRoles();
    };

    return (
        <Box>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
                <Typography variant="h5">Roles</Typography>
                <Button
                    variant="contained"
                    startIcon={<AddIcon />}
                    onClick={() => {
                        setEditMode(false);
                        setOpenDialog(true);
                    }}
                >
                    Add Role
                </Button>
            </Box>

            <TableContainer component={Paper}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>Name</TableCell>
                            <TableCell>Arabic Name</TableCell>
                            <TableCell align="right">Actions</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {roles.map((role) => (
                            <TableRow key={role.id}>
                                <TableCell>{role.name}</TableCell>
                                <TableCell>{role.translatedName}</TableCell>
                                <TableCell align="right">
                                    <IconButton
                                        color="primary"
                                        onClick={() => handleEditRole(role)}
                                    >
                                        <EditIcon />
                                    </IconButton>
                                    <IconButton
                                        color="error"
                                        onClick={() => handleDeleteRole(role.name)}
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
                <DialogTitle>{editMode ? 'Edit Role' : 'Create New Role'}</DialogTitle>
                <DialogContent>
                    <TextField
                        autoFocus
                        margin="dense"
                        label="Role Name (English)"
                        fullWidth
                        value={roleData.name}
                        onChange={(e) => setRoleData({
                            ...roleData,
                            name: e.target.value,
                            translations: {
                                ...roleData.translations,
                                en: e.target.value
                            }
                        })}
                    />
                    <TextField
                        margin="dense"
                        label="Role Name (Arabic)"
                        fullWidth
                        value={roleData.translations.ar}
                        onChange={(e) => setRoleData({
                            ...roleData,
                            translations: {
                                ...roleData.translations,
                                ar: e.target.value
                            }
                        })}
                    />
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setOpenDialog(false)}>Cancel</Button>
                    <Button onClick={handleCreateOrUpdateRole} variant="contained">
                        {editMode ? 'Update' : 'Create'}
                    </Button>
                </DialogActions>
            </Dialog>
        </Box>
    );
}; 