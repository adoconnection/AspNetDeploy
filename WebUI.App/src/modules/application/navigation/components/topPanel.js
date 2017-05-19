import {connect} from 'react-redux';
import {componentDidMount} from 'react-lifecycle-decorators';

import {Navbar, Nav, NavItem, MenuItem, NavDropdown} from 'react-bootstrap';

let TopPanel = ({router}) => (
    <Navbar inverse collapseOnSelect>
        <Navbar.Header>
            <Navbar.Brand>
                <NavItem>ASP.NET Deploy</NavItem>
            </Navbar.Brand>
            <Navbar.Toggle />
        </Navbar.Header>
        <Navbar.Collapse>
            <Nav>
                <NavItem eventKey={1} href="/App/Sources/List">Sources</NavItem>
            </Nav>
        </Navbar.Collapse>
    </Navbar>
);

TopPanel = componentDidMount(
    (props) => {
    }
)(TopPanel);

export default connect(
    (state) => {
        return state;
    }
)(TopPanel);


