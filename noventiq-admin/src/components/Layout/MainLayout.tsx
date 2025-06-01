import React from 'react';
import {
    AppBar,
    Box,
    CssBaseline,
    Drawer,
    IconButton,
    List,
    ListItem,
    ListItemIcon,
    ListItemText,
    Toolbar,
    Typography,
    useTheme,
} from '@mui/material';
import {
    Menu as MenuIcon,
    ChevronLeft as ChevronLeftIcon,
    People as PeopleIcon,
    VpnKey as VpnKeyIcon,
    Logout as LogoutIcon,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../../stores/auth.store';

const drawerWidth = 240;

interface MainLayoutProps {
    children: React.ReactNode;
}

export const MainLayout: React.FC<MainLayoutProps> = ({ children }) => {
    const theme = useTheme();
    const [open, setOpen] = React.useState(true);
    const navigate = useNavigate();
    const logout = useAuthStore(state => state.logout);
    const user = useAuthStore(state => state.user);

    const handleDrawerToggle = () => {
        setOpen(!open);
    };

    const handleLogout = async () => {
        await logout();
        navigate('/login');
    };

    const menuItems = [
        { text: 'Users', icon: <PeopleIcon />, path: '/users' },
        { text: 'Roles', icon: <VpnKeyIcon />, path: '/roles' },
    ];

    return (
        <Box sx={{ display: 'flex' }}>
            <CssBaseline />
            <AppBar
                position="fixed"
                sx={{
                    zIndex: theme.zIndex.drawer + 1,
                    transition: theme.transitions.create(['width', 'margin'], {
                        easing: theme.transitions.easing.sharp,
                        duration: theme.transitions.duration.leavingScreen,
                    }),
                    ...(open && {
                        marginLeft: drawerWidth,
                        width: `calc(100% - ${drawerWidth}px)`,
                        transition: theme.transitions.create(['width', 'margin'], {
                            easing: theme.transitions.easing.sharp,
                            duration: theme.transitions.duration.enteringScreen,
                        }),
                    }),
                }}
            >
                <Toolbar>
                    <IconButton
                        color="inherit"
                        aria-label="open drawer"
                        onClick={handleDrawerToggle}
                        edge="start"
                        sx={{ marginRight: 5 }}
                    >
                        <MenuIcon />
                    </IconButton>
                    <Typography variant="h6" noWrap component="div" sx={{ flexGrow: 1 }}>
                        Noventiq Admin
                    </Typography>
                    <Typography variant="body1" sx={{ mr: 2 }}>
                        {user?.email}
                    </Typography>
                    <IconButton color="inherit" onClick={handleLogout}>
                        <LogoutIcon />
                    </IconButton>
                </Toolbar>
            </AppBar>
            <Drawer
                variant="permanent"
                open={open}
                sx={{
                    width: drawerWidth,
                    flexShrink: 0,
                    whiteSpace: 'nowrap',
                    boxSizing: 'border-box',
                    ...(open && {
                        ...{
                            width: drawerWidth,
                            transition: theme.transitions.create('width', {
                                easing: theme.transitions.easing.sharp,
                                duration: theme.transitions.duration.enteringScreen,
                            }),
                            overflowX: 'hidden',
                        },
                    }),
                    ...(!open && {
                        transition: theme.transitions.create('width', {
                            easing: theme.transitions.easing.sharp,
                            duration: theme.transitions.duration.leavingScreen,
                        }),
                        overflowX: 'hidden',
                        width: theme.spacing(7),
                    }),
                }}
            >
                <Toolbar />
                <List>
                    {menuItems.map((item) => (
                        <ListItem
                            button
                            key={item.text}
                            onClick={() => navigate(item.path)}
                            sx={{
                                minHeight: 48,
                                justifyContent: open ? 'initial' : 'center',
                                px: 2.5,
                            }}
                        >
                            <ListItemIcon
                                sx={{
                                    minWidth: 0,
                                    mr: open ? 3 : 'auto',
                                    justifyContent: 'center',
                                }}
                            >
                                {item.icon}
                            </ListItemIcon>
                            <ListItemText
                                primary={item.text}
                                sx={{ opacity: open ? 1 : 0 }}
                            />
                        </ListItem>
                    ))}
                </List>
            </Drawer>
            <Box
                component="main"
                sx={{
                    flexGrow: 1,
                    p: 3,
                    width: { sm: `calc(100% - ${drawerWidth}px)` },
                }}
            >
                <Toolbar />
                {children}
            </Box>
        </Box>
    );
}; 